using System;

namespace core
{
	public interface IBlauSpace 
	{
		string HashedName {
			get;
		}
		
		int Dimension {
			get;
		}
		
		IBlauSpaceAxis getAxis(int i);
		bool hasAxis(string name);
		int getAxisIndex(string name);
		
		IBlauPoint getMinimalPoint();
	}
}

