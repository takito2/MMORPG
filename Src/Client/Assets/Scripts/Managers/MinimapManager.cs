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
        public UIMinimap minimap;

        public Collider minimapBoundingBox;
        public Collider MinimapBoundingBox
        {
            get { return minimapBoundingBox; }
        }



        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacter == null)
                    return null;
                return User.Instance.CurrentCharacterObject.transform;               
            }
        }
        public Sprite LoadCurrentMinimap()
        {
            Debug.Log("Minimap:" + User.Instance.CurrentMapData.MiniMap + "other:" + User.Instance.CurrentMapData.Resource);
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }

        public void UpdateMinimap(Collider minimapBoundingBox)
        {
            this.minimapBoundingBox = minimapBoundingBox;
            if (this.minimap != null)
                this.minimap.UpdateMap();
        }
    }
}
