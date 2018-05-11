using System;
using System.Collections.Generic;

namespace core
{
	public interface IBlauSpaceConfig
	{
		int Dimensions {
			get;
		}
		List<IBlauAxisConfig> getAxes();
	}
}

