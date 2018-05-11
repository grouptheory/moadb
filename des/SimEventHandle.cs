using System;
using core;

namespace des
{
    public class SimEventHandle : ISimEventHandle
	{
        private ISimEntity _sender, _target;
        private ISimEvent _simEvent;
        private UniqueDouble _udt;

        public SimEventHandle(ISimEntity sender, ISimEntity target, ISimEvent simEvent, UniqueDouble udt) 
		{
            _sender = sender;
            _target = target;
            _simEvent = simEvent;
          	_udt = udt;
      	}
		
		public ISimEntity Sender
		{
			get{return _sender;}
		}

		public ISimEntity Target
		{
			get{return _target;}
		}

		public IUniqueDouble UDT
		{
			get{return _udt;}
		}

		public ISimEvent Sim_Event
		{
			get{return _simEvent;}
		}
	}
}

