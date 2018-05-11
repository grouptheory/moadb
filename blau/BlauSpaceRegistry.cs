using System;
using System.Collections.Generic;
using core;
using logger;

namespace blau
{
	/// <summary>
    /// BlauSpaceRegistry class
    /// </summary>
	public class BlauSpaceRegistry
	{
		// singleton
		private static BlauSpaceRegistry _instance;
		
		// dictionary of blauspaces, keyed by hashed name
		private Dictionary<string, IBlauSpace> _spaces;
		
		// lazy singleton constructor
		public static BlauSpaceRegistry Instance() {
			if (_instance == null) {
				_instance = new BlauSpaceRegistry();
			}
			return _instance;
		}
		
		// getter which swaps in pre-existing identical blauspace, if one exists
		public IBlauSpace validate(IBlauSpace s) {
			if ( ! _spaces.ContainsKey(s.HashedName)) {
				add (s);
				return s;
			}
			else {
				return _spaces[s.HashedName];
			}
		}
		
		// add a blauspace lattice to the registry
		public void add(IBlauSpace s) {
			if ( ! _spaces.ContainsKey(s.HashedName)) {
				
				if (LoggerDiags.Enabled) SingletonLogger.Instance().DebugLog(typeof(BlauSpace), "BlauSpaceRegistry is adding "+s.HashedName);
				_spaces.Add (s.HashedName, s);
				if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(BlauSpaceLatticeRegistry), "BlauSpaceRegistry size is now "+_spaces.Count);
			}
		}
		
		// private constructor
		private BlauSpaceRegistry ()
		{
			_spaces = new Dictionary<string, IBlauSpace>();
		}
	}
}

