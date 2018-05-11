using System;
using core;

namespace des
{
	public abstract class SimEntity : ISimEntity
	{
	    protected SimEntity() 
		{
	        Scheduler.Instance().BirthSimEnt(this);
	    }
	
	    public void Kill() 
		{
	        this.Destructor();
	        Scheduler.Instance().KillSimEnt(this);
	    }
	
	    protected ISimEventHandle Send(ISimEntity dst, ISimEvent simEvent, double t) 
		{
	        return Scheduler.Instance().Register(this, dst, simEvent, t);
	    }
	
	    protected void RevokeSend(ISimEventHandle eventHandle) 
		{
	        Scheduler.Instance().Deregister(eventHandle);
	    }
	
	    public abstract void Destructor();
		
	    public abstract string GetName();
	
	    public abstract void Recv(ISimEntity src, ISimEvent simEvent);
	
	    public void DeliveryAck(ISimEventHandle eventHandle) 
		{
	        // default no-op
	    }
	}
}

