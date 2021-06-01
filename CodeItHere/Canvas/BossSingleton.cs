using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas
{
    public class BossSingleton
    {
        public string Name { get; set; }
        static Lazy<BossSingleton> _instance = new Lazy<BossSingleton>(()=>new BossSingleton(""));
        private BossSingleton(string name)
        {
            Name = name;
        }

        private BossSingleton Init(string name)
        {
            return new BossSingleton(name);
        }
        public static BossSingleton GetBoss(string name)
        {
            if (_instance.Value == null)
            {
                _instance.Value.Name = name;
            }
            return _instance.Value;
        }
    }
}
