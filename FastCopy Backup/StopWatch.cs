using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StopWatchTest
{

    public class StopWatch

    {

        DateTime _starttime;

        DateTime _stoptime;

        bool work;

 

        public StopWatch()

        {

            _starttime = new DateTime();

            _stoptime = new DateTime();

            work = false;

        }

 

        public void Start()

        {

            if(!work)

            {

                _starttime = DateTime.Now;

                work = true;

            }

            else throw new InvalidOperationException();

        }

 

        public void Stop()

        {

            if(work)

            {

                _stoptime = DateTime.Now;

 

                work = false;    

            }

            else throw new InvalidOperationException();

        }

 

        public void Reset()

        {

            _starttime = new DateTime();

            _stoptime = new DateTime();

            work = false;

        }

 

        public TimeSpan LapTime

        {

            get

            {

                if(work)

                {

                    return DateTime.Now - _starttime;//動いていたら現在の時間から計算

                }

                else throw new InvalidOperationException();

            }

        }

 

        public TimeSpan TimeSpan

        {

            get

            {

                if(work)

                {

                    return (DateTime.Now - _starttime);

                }

                else

                {

                    return (_stoptime - _starttime);

                }            

            }

        }

 

        public static TimeSpan operator+(StopWatch x, StopWatch y)

        {

            return x.TimeSpan + y.TimeSpan;

        }

 

        public static TimeSpan operator-(StopWatch x, StopWatch y)

        {

            return x.TimeSpan - y.TimeSpan;

        }

 

        public override string ToString()

        {

            return this.TimeSpan.ToString();

        }

    }

}
