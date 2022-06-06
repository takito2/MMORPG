using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Entities;
using Services;
using Models;

namespace Managers
{
    class MinimapManager :Singleton<MinimapManager>
    {
        public Sprite LoadCurrentMinimap()
        {
            Debug.Log("Minimap:" + User.Instance.CurrentMapData.MiniMap + "other:" + User.Instance.CurrentMapData.Resource);
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }
    }
}
