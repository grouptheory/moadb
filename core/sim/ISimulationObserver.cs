using System;

namespace core
{
	public interface ISimulationObserver
	{
		void recvSimulationNotification(ISimulationParameters sim, ISimulationEvent evt);
	}
}

