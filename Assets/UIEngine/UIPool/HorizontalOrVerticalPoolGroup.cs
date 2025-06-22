﻿using UnityEngine;
using System.Collections.Generic;
using GIKCore.Utilities;

namespace UIEngine.UIPool
{
    public abstract class HorizontalOrVerticalPoolGroup : BasePoolGroup
    {
        // Method	

        /// <summary>
        /// <para>In case of multiple size, return cell size you want to work with data at 'index' in adapter.</para>
        /// <para></para>
        /// <para>Param: delegate (int 'index of data in adapter')</para>
        /// </summary>
        public void SetCellSizeCallback(ICallback.CallFunc6<Vector2, int> func) { cellSizeCB = func; }       

        /// <summary>
        /// <para>In case of multiple size, return size of GameObject have data at 'index' in adapter.</para>
        /// <para></para>
        /// <para>Param: Index of data in adapter</para>
        /// </summary>
        protected Vector2 GetElementSize(int index)
        {
            if (cellSizeCB != null)
                return cellSizeCB(index);
            return GetCellSize();
        }

        protected void CheckToAddAtleast3Cells()
        {
            int numElement = adapter.Count;
            if (numElement <= 0)
                return;

            int _1stIndex = 0;
            int _2ndIndex = 1;
            int _3rdIndex = 2;

            bool _1stFound = false;
            bool _2ndFound = false;
            bool _3rdFound = false;

            foreach (PoolObject po in listPool)
            {
                if (po.index == _1stIndex) _1stFound = true;
                else if (po.index == _2ndIndex) _2ndFound = true;
                else if (po.index == _3rdIndex) _3rdFound = true;
            }

            if (_2ndIndex > (numElement - 1)) _2ndFound = true;
            if (_3rdIndex > (numElement - 1)) _3rdFound = true;

            if (!_1stFound) GetPooledObject(_1stIndex);
            if (!_2ndFound) GetPooledObject(_2ndIndex);
            if (!_3rdFound) GetPooledObject(_3rdIndex);
        }

        public override BasePoolGroup SetAdapter<T>(List<T> adapter, bool resetPosition = true)
        {
            base.SetAdapter(adapter, resetPosition);
            CalculateSizeDelta();
            ResetPool();
            CheckToAddAtleast3Cells();
            UpdateData();

            if (resetPosition) ScrollToFirst(0);
            return this;
        }        
    }
}