using System;
using System.Collections.Generic;

namespace core
{
	public interface IBlauPointConfig
	{
		int Dimensions {
			get;
		}
		
		List<IBlauCoordConfig> getCoords();
	}
}

