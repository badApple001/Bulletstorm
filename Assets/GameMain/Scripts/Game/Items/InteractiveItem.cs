using UnityEngine;

public class InteractiveItem : MonoBehaviour
{
    public enum ItemType
    {
        /// <summary>
        /// 消耗品
        /// </summary>
        Consumables,

        /// <summary>
        /// 武器
        /// </summary>
        Weapon,

        /// <summary>
        /// 金币
        /// </summary>
        GoldCoin,


        /// <summary>
        /// 特殊物品可以用来后续扩展, 在特殊物品类里处理多态来维护后期的活动道具需求
        /// </summary>
        SpecialGoods,
    }


}
