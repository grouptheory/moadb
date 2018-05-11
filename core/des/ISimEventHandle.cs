using System;

namespace core
{
	public interface ISimEventHandle
	{
		ISimEntity Sender
		{
			get;
		}

		ISimEntity Target
		{
			get;
		}

		IUniqueDouble UDT
		{
			get;
		}

		ISimEvent Sim_Event
		{
			get;
		}
	}
}

