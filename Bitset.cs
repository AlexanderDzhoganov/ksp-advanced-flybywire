using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    public class Bitset
    {

        private List<int> m_Data = new List<int>();

        private int m_NumBits = 0;

        public Bitset(int numBits)
        {
            m_NumBits = numBits;

            int numInts = numBits / 32;

            for (var i = 0; i < numInts; i++)
            {
                m_Data.Add(0);
            }
        }

        public Bitset(int numBits, int initialValue)
        {
            m_NumBits = numBits;

            int numInts = numBits / 32;

            for (var i = 0; i < numInts; i++)
            {
                m_Data.Add(0);
            }

            m_Data[0] = initialValue;
        }

        public void Set(int bit, bool state = true)
        {
            if (bit >= m_NumBits)
            {
                return;
            }

            int pos = bit / 32;
            int posLocal = bit - 32 * pos;
            m_Data[pos] |= 1 << posLocal;
        }

        public bool Get(int bit)
        {
            if(bit >= m_NumBits)
            {
                return false;
            }

            int pos = bit / 32;
            int posLocal = bit - 32 * pos;
            return (m_Data[pos] & (1 << posLocal)) != 0;
        }
    
    }

}
