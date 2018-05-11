using System;
using System.Collections;
//using System.Collections.Generic;
using System.Diagnostics;
using core;
using logger;
using C5;

namespace des
{
	public class Scheduler
	{
	    private static Scheduler _instance;
	    public static Scheduler Instance() 
		{
	        if (_instance == null) { _instance = new Scheduler(); }
	        return _instance;
	    }
		
	    private C5.HashDictionary<ISimEntity,C5.HashSet<ISimEventHandle>> _from2set;
	    private C5.HashDictionary<ISimEntity,C5.HashSet<ISimEventHandle>> _to2set;
		// UniqueDouble(time)->Event
	    private C5.TreeDictionary<IUniqueDouble,ISimEventHandle> _ud2ehandle;
		
	    private bool _done = false;
	    private double _timeNow = 0;
		
		// registrations for the same time.
    	private int _uid=0;
		
	    private Scheduler() {
			_from2set = new C5.HashDictionary<ISimEntity,C5.HashSet<ISimEventHandle>>();
			_to2set = new C5.HashDictionary<ISimEntity,C5.HashSet<ISimEventHandle>>();
			
			_ud2ehandle = new C5.TreeDictionary<IUniqueDouble,ISimEventHandle>();
			_done = false;
	    	_timeNow = 0;
		}
	
	    public void Reset() 
		{
			_from2set = new C5.HashDictionary<ISimEntity,C5.HashSet<ISimEventHandle>>();
			_to2set = new C5.HashDictionary<ISimEntity,C5.HashSet<ISimEventHandle>>();
			
			_ud2ehandle = new C5.TreeDictionary<IUniqueDouble,ISimEventHandle>();
			_done = false;
	    	_timeNow = 0;
			/*
	        this._from2set.Clear();
	        this._to2set.Clear();
	        this._ud2ehandle.Clear();
	        this._timeNow = 0;
	        this._uid = 0;
	        this._done = false;
	        */
	    }
		
	    private HashSet<ISimEventHandle> GetEventsFrom(ISimEntity simEntity) 
		{
	        HashSet<ISimEventHandle> hSet;
			if (_from2set.Contains(simEntity)) {
				hSet = (HashSet<ISimEventHandle>)_from2set[simEntity];
			}
			else
			{
	           hSet = new HashSet<ISimEventHandle>();
	           _from2set.Add(simEntity, hSet);
	        }
	        return hSet;
	    }
	
	    private HashSet<ISimEventHandle> GetEventsTo(ISimEntity simEntity) 
		{
	        HashSet<ISimEventHandle> hSet;
			if (_to2set.Contains(simEntity)) {
				hSet = (HashSet<ISimEventHandle>)_to2set[simEntity];
			}
			else
			{
	            hSet = new HashSet<ISimEventHandle>();
	            _to2set.Add(simEntity, hSet);
	        }
	        return hSet;
	    } 
	    
	    public ISimEventHandle Register(ISimEntity sender, ISimEntity target, ISimEvent simEvent, double t) 
		{
	        if (t < 0) 
			{
	            Console.WriteLine("Cannot register an event in the past!");
	             System.Diagnostics.StackTrace st = new StackTrace(true);
				Console.WriteLine(st.ToString());
				Environment.Exit(-1);
	        }
	        
			SingletonLogger.Instance().DebugLog(typeof(Scheduler), "@ "+Scheduler.GetTime()+" Register src:"+sender+" dst:"+target+" ev:"+simEvent+" t:"+t);
			
			double deliveryTime = Scheduler.GetTime() + t;
	        ISimEventHandle eventHandle = new SimEventHandle(sender, target, simEvent, new UniqueDouble(deliveryTime));
			
	        HashSet<ISimEventHandle> eventsFrom = Instance().GetEventsFrom(eventHandle.Sender); 
			eventsFrom.Add(eventHandle);
			
	        HashSet<ISimEventHandle> eventsTo = Instance().GetEventsTo(eventHandle.Target);
			eventsTo.Add(eventHandle);
			
			Instance()._ud2ehandle.Add(eventHandle.UDT, eventHandle);
	        
			return eventHandle;
	    }
	
	    public void Deregister(ISimEventHandle eventHandle) 
		{
	        Instance().GetEventsFrom(eventHandle.Sender).Remove(eventHandle);
	        Instance().GetEventsTo(eventHandle.Target).Remove(eventHandle);
	        Instance()._ud2ehandle.Remove(eventHandle.UDT);
	    }
	
	    public void Stop() 
		{ 
			_done = true; 
		}
		
		public void InitializeStartTime(double startTime)
		{
			_timeNow = startTime;
		}
	
		public int EventCounter {
			get { return _ud2ehandle.Count; }
		}
		
	    public void Run() 
		{
			SingletonLogger.Instance().DebugLog(typeof(Scheduler), "Scheduler: At start of Run() _ud2ehandle.Size(): "+_ud2ehandle.Count);
			
			_done = false;
			
	        do 
			{
	            if (_ud2ehandle.Count == 0) _done = true;
	            else 
				{
					KeyValuePair<IUniqueDouble, ISimEventHandle> kvp = _ud2ehandle.FindMin();
	                IUniqueDouble udt = kvp.Key;
	                ISimEventHandle eventHandle = kvp.Value;
					
	                _timeNow = udt.Value;
					
	                eventHandle.Sim_Event.Entering(eventHandle.Target);
	                eventHandle.Target.Recv(eventHandle.Sender, eventHandle.Sim_Event);
	                eventHandle.Sender.DeliveryAck( eventHandle );
					
	                Deregister(eventHandle);
	            }
	        }
	        while (!_done);
			KillAll();
			
			Reset ();
			SingletonLogger.Instance().DebugLog(typeof(Scheduler), "Scheduler: At end of Run() _ud2ehandle.Size(): "+_ud2ehandle.Count);
	    }
	
	    public static double GetTime() 
		{ 
			return Instance()._timeNow; 
		}

	    public void KillSimEnt(SimEntity simEntity) 
		{
	        // clone to avoid concurrent modifications from deregister
	        HashSet<ISimEventHandle> fromHandles = new HashSet<ISimEventHandle>();
			fromHandles.AddAll<ISimEventHandle>(GetEventsFrom(simEntity));
	        foreach (ISimEventHandle eventHandle in fromHandles) 
			{
	            Deregister(eventHandle);
	        }
			
	        _from2set.Remove(simEntity);
	
	        // clone to avoid concurrent modifications from deregister
	        HashSet<ISimEventHandle> toHandles = new HashSet<ISimEventHandle>();
	        toHandles.AddAll<ISimEventHandle>(GetEventsTo(simEntity));
			foreach (ISimEventHandle eventHandle in toHandles) 
			{
	            Deregister(eventHandle);
	        }
	        _to2set.Remove(simEntity);
	    }
	
	    public void BirthSimEnt(SimEntity simEntity) 
		{
	        // make the sets by getting them
	        HashSet<ISimEventHandle> fromHandles = Instance().GetEventsFrom(simEntity);
	        HashSet<ISimEventHandle> toHandles = Instance().GetEventsTo(simEntity);
			if(fromHandles.Count == toHandles.Count)
			{
				//dummy if warning removal	
			}
	    }
	
	    private void KillAll() 
		{
	        while (_from2set.Keys.Count > 0) 
			{
	            SimEntity se = null;
	            IEnumerator enumerator = _from2set.Keys.GetEnumerator();
	            enumerator.MoveNext();
				se = (SimEntity) enumerator.Current;
	            se.Kill();
	        }
	    }
		
		public int UID
		{
			get{return _uid;}
			set{_uid = value;}
		}

	}
}

