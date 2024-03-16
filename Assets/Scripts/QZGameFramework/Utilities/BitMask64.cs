using System;

namespace QZGameFramework.Utilities
{
    internal struct BitMask64
    {
        private long mask;

        public static implicit operator long(BitMask64 mask)
        { return mask.mask; }

        public static implicit operator BitMask64(long mask)
        { return new BitMask64(mask); }

        public BitMask64(long mask)
        {
            this.mask = mask;
        }

        /// <summary>
        /// 打开位
        /// </summary>
        public void Open(int bit)
        {
            if (bit < 0 || bit > 63)
                throw new ArgumentOutOfRangeException();
            else
                mask |= 1L << bit;
        }

        /// <summary>
        /// 关闭位
        /// </summary>
        public void Close(int bit)
        {
            if (bit < 0 || bit > 63)
                throw new ArgumentOutOfRangeException();
            else
                mask &= ~(1L << bit);
        }

        /// <summary>
        /// 位取反
        /// </summary>
        public void Reverse(int bit)
        {
            if (bit < 0 || bit > 63)
                throw new ArgumentOutOfRangeException();
            else
                mask ^= 1L << bit;
        }

        /// <summary>
        /// 所有位取反
        /// </summary>
        public void Inverse()
        {
            mask = ~mask;
        }

        /// <summary>
        /// 比对位值
        /// </summary>
        public bool Test(int bit)
        {
            if (bit < 0 || bit > 63)
                throw new ArgumentOutOfRangeException();
            else
                return (mask & (1L << bit)) != 0;
        }
    }
}