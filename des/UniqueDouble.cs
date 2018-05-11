using System;
using core;

namespace des
{
	public class UniqueDouble : IUniqueDouble
	{
		double _value;
        int _discriminator;

        public UniqueDouble(double value) 
		{
            _value = value;
            _discriminator = (Scheduler.Instance().UID);
            (Scheduler.Instance().UID)++;
        }

        public int CompareTo(object obj) 
		{
            UniqueDouble other = (UniqueDouble)obj;
			
			// compare by value
            if (this._value < other._value)
                return -1;
            else if (this._value > other._value)
                return +1;
            else 
			{ // but use the discriminator if the values agree
                if (this._discriminator < other._discriminator)
                    return -1;
                else if (this._discriminator > other._discriminator)
                    return +1;
                else
                    return 0;
            }
		}
		
		public double Value
		{
			get {return _value;}
		}
		
		override public string ToString() 
		{
            return _value.ToString()+"("+_discriminator+")";
        }
	}
}

