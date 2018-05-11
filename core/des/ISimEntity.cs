using System;

namespace core
{
	public interface ISimEntity
	{
	    void Kill();
	
	    void Destructor();
		
	    string GetName();
	
	    void Recv(ISimEntity src, ISimEvent simEvent);
	
	    void DeliveryAck(ISimEventHandle eventHandle);
	}
}

