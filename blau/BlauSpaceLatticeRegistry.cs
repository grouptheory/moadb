using System;
using System.Collections.Generic;
using core;
using logger;

namespace blau
{
	/// <summary>
    /// BlauSpaceLatticeRegistry class
    /// </summary>
	public class BlauSpaceLatticeRegistry
	{
		// singleton
		private static BlauSpaceLatticeRegistry _instance;
		
		// lazy singleton constructor
		public static BlauSpaceLatticeRegistry Instance() {
			if (_instance == null) {
				_instance = new BlauSpaceLatticeRegistry();
			}
			return _instance;
		}
		
		// dictionary of blauspace lattices, keyed by hashed name
		private Dictionary<string, BlauSpaceLattice> _lattices;
		
		// getter which swaps in pre-existing identical blauspace lattice, if one exists
		public BlauSpaceLattice validate(BlauSpaceLattice s) {
			if ( ! _lattices.ContainsKey(s.HashedName)) {
				return s;
			}
			else {
				return _lattices[s.HashedName];
			}
		}
		
		// add a blauspace lattice to the registry
		public void add(BlauSpaceLattice s) {
			if ( ! _lattices.ContainsKey(s.HashedName)) {
				if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(BlauSpaceLatticeRegistry), "BlauSpaceLatticeRegistry is adding "+s.HashedName);
				_lattices.Add (s.HashedName, s);
				if (LoggerDiags.Enabled) SingletonLogger.Instance().InfoLog(typeof(BlauSpaceLatticeRegistry), "BlauSpaceLatticeRegistry size is now "+_lattices.Count);
			}
		}
		
		// private constructor
		private BlauSpaceLatticeRegistry ()
		{
			_lattices = new Dictionary<string, BlauSpaceLattice>();
		}
	}
}

