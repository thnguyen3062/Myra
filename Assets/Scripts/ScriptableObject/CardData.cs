using GIKCore.Bundle;
using GIKCore.Lang;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Data/CardData")]
public class CardData : ScriptableObject
{
    private static CardData instance;
    public static CardData Instance
    {
        get
        {
            if (instance == null)
            {
                return instance = Resources.Load<CardData>("Data/CardData");
            }
            return instance;
        }
    }

    public List<CardSkillDataInfo> skillDataInfo;
    public List<SkillSound> skillSound;

    public Sprite GetOnBoardSprite(long id)
    {
        if(Database.GetHero(id) == null)
        {
            Debug.Log("heroId: " + id);
        }
        Sprite targetSprite = BundleHandler.main.GetSprite("onboard_" + Database.GetHero(id).color, id.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/CardOnBoard_" + Database.GetHero(id).color + "/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetUltiSprite(string id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("ultimateskillnew", id.ToString());
        if (targetSprite == null)
        {
            //Sprite dataNext = Resources.Load<Sprite>("Pack/Images/UltimateSkill/" + id);
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/UltimateSkillNew/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetGodIconSprite(string id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("godicon", id.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/GodIcon/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Texture GetGodIconTexture(string id)
    {
        Texture targetSprite = BundleHandler.main.GetTexture("godicon", id.ToString());
        if (targetSprite == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/GodIcon/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetGodCardSprite(long id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("godava", id.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/GodAva/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetFilterCardSprite(long id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("cardfilter", id.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/FilterCard/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetRaritySprite(long rarity)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("cardrarity", rarity.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Rarity/" + rarity);
            if (dataNext != null)
            {
                return dataNext;
            }
        }
        //LogWriterHandle.WriteLog("No Target With ID " + rarity + " Image Found");
        return targetSprite;
    }
    public Texture GetOnBoardTexture(long id)
    {
        Texture targetTexture = BundleHandler.main.GetTexture("onboard_" + Database.GetHero(id).color, id.ToString());
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/CardOnBoard_" + Database.GetHero(id).color + "/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetTexture;
    }
    public Texture GetUltiTexture(string id)
    {
        Texture targetTexture = BundleHandler.main.GetTexture("ultimateskillnew", id.ToString());
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/UltimateSkillNew/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetTexture;
    }
    public Texture GetGodIconTexture(long id)
    {
        Texture targetTexture = BundleHandler.main.GetTexture("godicon", id.ToString());
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/GodIcon/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetTexture;
    }
    public Texture GetRarityTexture(long rarity)
    {
        Texture targetTexture = BundleHandler.main.GetTexture("cardrarity", rarity.ToString());
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/Rarity/" + rarity);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + rarity + " Image Found");
        return targetTexture;
    }
    public Sprite GetCardColorSprite(string color)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("cardcolor", color.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/CardColor/" + color);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + color + " Image Found");
        return targetSprite;
    }
    public Sprite GetCardFrameBlack(long type, long rarity)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("frame", string.Format("Black_Filter_{0}_{1}", type, rarity));
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Frame/" + string.Format("Black_Filter_{0}_{1}", type, rarity));
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + color + " Image Found");
        return targetSprite;
    }
    public Sprite GetCardFrameSelect(long type, long rarity)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("frame", string.Format("Select_{0}_{1}", type, rarity));
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Frame/" + string.Format("Select_{0}_{1}", type, rarity));
            if (dataNext != null) 
                return dataNext;
        }
        return targetSprite;
    }
    public Sprite GetCardFrameNew(long type, long rarity)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("frame", string.Format("New_{0}_{1}", type, rarity));
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Frame/" + string.Format("New_{0}_{1}", type, rarity));
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + color + " Image Found");
        return targetSprite;
    }
    public Sprite GetCardFrameSprite(string frame)

         
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("frame", frame.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Frame/" + frame);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + frame + " Image Found");
        return targetSprite;
    }
    public Texture GetCardColorTexture(string color)
    {
        Texture targetTexture = BundleHandler.main.GetTexture("cardcolor", color.ToString());
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/CardColor/" + color);
            if (dataNext != null)
                return dataNext;
        } 
        return targetTexture;
    }
    public Texture GetCardFrameTexture(string frame)  
    {
        Texture targetTexture = BundleHandler.main.GetTexture("frame", frame.ToString());
        //Debug.Log(targetTexture.name);
        //Texture targetTexture = null;
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/Frame/" + frame);
            if (dataNext != null)  
                return dataNext;
        }
        return targetTexture;
    }
    public Texture GetCardFrameMaskTexture(string frame)
    {
        //M(Type)_??Hi?m
        Texture targetTexture = BundleHandler.main.GetTexture("frame", frame.ToString());
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/Frame/" + frame);
            if (dataNext != null)
                return dataNext;
        }
        return targetTexture;
    }
    public Sprite GetCardFrameMaskSprite(string frame)
    {
        //M(Type)_??Hi?m
        Sprite targetTexture = BundleHandler.main.GetSprite("frame", frame.ToString());
        if (targetTexture == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Frame/" + frame);
            if (dataNext != null)
                return dataNext;
        }
        return targetTexture;
    }
    public Texture GetCardFrameMaskTextureCanvas(int type, long rarity)
    {
        Texture targetTexture = BundleHandler.main.GetTexture("frame", string.Format("M_{0}_{1}", type, rarity));
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/Frame/" + string.Format("M_{0}_{1}", type, rarity));
            if (dataNext != null)
                return dataNext;
        }
        return targetTexture;
    }
    public Texture GetRemandBlueCrystals()
    {
        Texture targetTexture = BundleHandler.main.GetTexture("materials", "RedandBlueCrystals");
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Materials/" + "RedandBlueCrystals");
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + "RedandBlueCrystals" + " Image Found");
        return targetTexture;
    }
    public Sprite GetGodFullSprite(long id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("godfull", id.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/GodFull/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetGodCardNewSprite(long id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("godimages", id.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/God_" + Database.GetHero(id).color + "/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetGodDeckSprite(long id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("goddeck", id.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/GodDeck/" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetRankSprite(string id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("rank", id);
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Rank/" + id);
            if (dataNext != null)
                return dataNext;
        }
        LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetDeckFrameSprite(int count, int id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("deckframe", id.ToString());
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/DeckFrame/god_" + count + "_" + id);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public VideoClip GetVideo(string videoName)
    {
        VideoClip targetVideo = BundleHandler.main.GetVideo("videos", videoName);
        if (targetVideo == null)
        {
            VideoClip dataNext = Resources.Load<VideoClip>("Videos/" + videoName);
            if (dataNext != null)
                return dataNext;
        }
        return targetVideo;
    }
    public Object[] AnimatedCardResource()
    {
        Object[] source = BundleHandler.main.GetAllObject("animatedmaterialsresouce");
        return source;
    }    
    public Material GetAnimatedMaterial(string materialName)
    {
        Material targetMat = BundleHandler.main.GetMaterial("animatedmaterials", materialName);
        //Material targetMat = null;
        if (targetMat == null)
        {
            Material dataNext = Resources.Load<Material>("Pack/AnimatedMaterials/" + materialName);
            if (dataNext != null)
            {
                return dataNext;
            } else
            {
            }
        }
        return targetMat;
    }
    public Material GetAnimatedMaterialBoardCard(string materialName)
    {
        Material targetMat = BundleHandler.main.GetMaterial("animatedmaterialsboardcard", materialName);
        //Material targetMat = null;
        if (targetMat == null)
        {
            Material dataNext = Resources.Load<Material>("Pack/AnimatedMaterialsBoardCard/" + materialName);
            if (dataNext != null)
            {
                return dataNext;
            }
            else
            {
            }
        }
        return targetMat;
    }    
    public Object GetUltiVfxPrefab(string id)
    {
        Object obj = BundleHandler.main.GetObject("ultivfx",id);
        //Object obj = null;
        if (obj == null)
        {
            Object dataNext = Resources.Load<Object>("Pack/Prefab/UltiVFX/"+ id);
            if (dataNext != null)
                return dataNext;
        }
        return obj;
    }    
    private List<CardDataInfo> lstCardDataInfo = new List<CardDataInfo>();
    private List<CardDataInfo> LstCardDataInfo
    {
        get
        {
            if (lstCardDataInfo.Count == 0)
            {
                string aJSON = BundleHandler.LoadDatabase("HeroInfo_" + LangHandler.lastType);
                JSONNode N = JSON.Parse(aJSON);
                JSONArray jArray = N.AsArray;

                for (int i = 0; i < jArray.Count; i++)
                {
                    JSONObject o = jArray[i].AsObject;
                    CardDataInfo info = new CardDataInfo();
                    info.id = o["id"].AsLong;
                    info.name = o["name"].Value;
                    JSONArray aSkillName = o["skillname"].AsArray;
                    for (int y = 0; y < aSkillName.Count; y++)
                    {
                        string txt = aSkillName[y].Value.Replace("\\", "");
                        info.skillName.Add(txt);
                    }
                    JSONArray aDesc = o["description"].AsArray;
                    for (int x = 0; x < aDesc.Count; x++)
                    {
                        string txt = aDesc[x].Value.Replace("\\", "");
                        info.description.Add(txt);
                    }
                    
                    lstCardDataInfo.Add(info);
                }
            }
            return lstCardDataInfo;
        }
    }

    public CardDataInfo GetCardDataInfo(long id)
    {
        CardDataInfo info = LstCardDataInfo.FirstOrDefault(x => x.id == id);
        if (info != null)
            return info;
        LogWriterHandle.WriteLog("No Card Info With ID " + id + " Found");
        return LstCardDataInfo[3];
    }
    private List<SkillAuctionDataInfo> listSkillAuctionInfo = new List<SkillAuctionDataInfo>();

    private List<SkillAuctionDataInfo> ListSkillBidInfo
    {
        get
        {
            if (listSkillAuctionInfo.Count == 0)
            {
                string aJSON = BundleHandler.LoadDatabase("BidSkill");
                
                JSONNode N = JSON.Parse(aJSON);
                JSONArray jArray = N.AsArray;

                for (int i = 0; i < jArray.Count; i++)
                {
                    JSONObject o = jArray[i].AsObject;
                    SkillAuctionDataInfo skill = new SkillAuctionDataInfo();
                    skill.skillID = o["id"].AsLong;
                    skill.descEN = o["en"].Value;
                    skill.descKR = o["kr"].Value;
                    skill.descJP = o["jp"].Value;
                    skill.descCN = o["cn"].Value;
                    skill.descTW = o["tw"].Value;

                    listSkillAuctionInfo.Add(skill);
                }
            }
            return listSkillAuctionInfo;
        }
    }
    public SkillAuctionDataInfo GetSkillBidInfo(long id)
    {
        SkillAuctionDataInfo info = ListSkillBidInfo.FirstOrDefault(x => x.skillID == id);
        if (info != null)
            return info;
        LogWriterHandle.WriteLog("No Skill Info With ID " + id + " Found");
        return null;
    }

    public void OnChangeLanguage()
    {
        lstCardDataInfo = new List<CardDataInfo>();
    }

    public CardSkillDataInfo GetCardSkillDataInfo(long id)
    {
        CardSkillDataInfo info = skillDataInfo.FirstOrDefault(x => x.heroID == id);
        if (info != null)
            return info;
        return null;
    }
    public SkillSound GetCardSkillSound(long id)
    {
        SkillSound info = skillSound.FirstOrDefault(x => x.skillIds.Contains(id));
        if (info != null)
        {
            return info;
        }    
        return null;
    }
    public Sprite GetShopItemGodFrame(long id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("frame", "ShopItemGodFrame_" + Database.GetHero(id).color);
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Frame/ShopItemGodFrame_" + Database.GetHero(id).color);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetShopItemNormalFrame(long id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("frame", "ShopItemNormalFrame_" + Database.GetHero(id).color);
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Frame/ShopItemNormalFrame_" + Database.GetHero(id).color);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Sprite GetShopItemGodUlti(long id)
    {
        Sprite targetSprite = BundleHandler.main.GetSprite("frame", "ShopItemGodUlti_" + Database.GetHero(id).color);
        if (targetSprite == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Frame/ShopItemGodUlti_" + Database.GetHero(id).color);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + id + " Image Found");
        return targetSprite;
    }
    public Material GetCardFrameMaterial(string frame)
    {
     //   Material test = BundleHandler.main.GetMaterial("frame",frame);
        Material targetMat = null;
        if (targetMat == null)
        {
            //string path = string.Format("Pack/MaterialsCard/Frame/M_{0}_{1}_{2}", type, frame,rarity);
            Material dataNext = Resources.Load<Material>("MaterialsCard/Frame/" + frame);
            return dataNext;
        }
        return targetMat;
    }
    public Texture GetTextureShopUI(string name)
    {
        Texture targetTexture = BundleHandler.main.GetTexture("shop", name);
        if (targetTexture == null)
        {
            Texture dataNext = Resources.Load<Texture>("Pack/Images/Shop/" + name );
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + "RedandBlueCrystals" + " Image Found");
        return targetTexture;
    }
    public Sprite GetSpriteShopUI(string name)
    {
        Sprite targetTexture = BundleHandler.main.GetSprite("shop", name);
        if (targetTexture == null)
        {
            Sprite dataNext = Resources.Load<Sprite>("Pack/Images/Shop/" + name);
            if (dataNext != null)
                return dataNext;
        }
        //LogWriterHandle.WriteLog("No Target With ID " + "RedandBlueCrystals" + " Image Found");
        return targetTexture;
    }
}
