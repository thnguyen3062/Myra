using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIParticleExtensions;

[System.Serializable]
public class LevelRequirement
{
    public int hp;
    public int required;
}

[System.Serializable]
public class LevelRequirementsWrapper
{
    public LevelRequirement[] levelRequirements;
}


public class SC_Bar_Nonstop : MonoBehaviour
{
    //Bar này hiện lên lúc Bonus bay vào cục Shard trên góc trái. Cộng số lượng bonus để lên level cục shard đến level cần lên. Sau đó biến mất.

    public int currentShards = 0; // Ví dụ về tổng số shard đang có, cần lấy từ service của Hương
    public int addShards; //số shard được cộng thêm, cần lấy từ service của Hương
    public TextAsset jsonFile; // JSON của Hương
    public SC_Info_Upgrade InfoScript;
    [SerializeField] private GameObject Bar; //Bar xanh blue
    [SerializeField] private TextMeshProUGUI amountText; //text: số shard đang có
    [SerializeField] private TextMeshProUGUI targetText; //text: số shard yêu cầu để lên cấp
    [SerializeField] private GameObject addShardAmount; //Số lượng được tăng lên //Cho animation bay lên từ bar.
    private TextMeshProUGUI addShardText; //text: số shard được add thêm
    private Color notFullColor = new Color(69f / 255f, 228f / 255f, 1f, 1f);
    private Color fullColor = new Color(45f / 255f, 1f, 1.3f / 255f, 1f);
    [SerializeField] private GameObject Glow; //Glow bar
    [SerializeField] private GameObject Arrow;
    private Color color;
    private Material barMaterial;
    private Material glowMaterial;
    private Image barImage;
    private Image glowImage;
    private Sequence barSeq;
    public bool isBarSeqPlaying = false; //Đang true thì không cho sequence chạy lại.
    private GameObject objectToDestroy; //Để destroy particle khi không dùng nữa.
    [SerializeField] private Transform shardIcon; //icon của shard
    [SerializeField] private GameObject shardIconGlow; //glow quanh icon shard
    [SerializeField] private GameObject shardBarBg; //background của shard
    [SerializeField] private GameObject EndParticleContainer_1; //prefab particle system ở shard
    private ParticleSystem endBurstParticle;
    private Material bgMaterial;
    private int totalShards;
    private int fakeInt; //để gửi event BonusFlyDone đúng mẫu với function kích hoạt ở script làm mờ canvas tổng của cả thanh shard
    public delegate void BonusFlyEventHandler(int fakeInt);
    public event BonusFlyEventHandler BonusFlyDone; //event khi chạy xong thì bắn, làm mờ canvas tổng
    private Sequence shardNumberSeq; //sequence hiện số shard được thêm, rồi mờ đi
    private Sequence ScaleIcon; //Sequence nảy shard icon khi có particle đập vào
    private int currentDistance = 0; //Khoảng cách giữa các bậc máu shard từ file JSON, đơn vị là số shard
    private float fillDuration = 1f; //thời gian chạy các animation mỗi vòng for
    private int startCount = 0; //dùng cho DOCounter, vị trí bắt đầu đếm lên.
    int startProgress; //Số shard đang có trong progress ở bậc máu shard ày vào thời điểm START
    int startDistance; //Số shard target ở bậc máu shard này vào thời điểm START
    private LevelRequirementsWrapper wrapper; //dùng để load json
    private int currentProgressNow; //progress hiện tại ở bậc máu shard hiện tại
    private int currentIndex; //currentIndex là level hiện tại, vòng for thì i sẽ bắt đầu từ đây, tránh chạy anim cho các level trước đã qua.
    [SerializeField] private TextMeshProUGUI towerHpText; //text: máu shard
    [SerializeField] private Transform particleLocation;



    void Start()
    {
        

    }
    public void SetData(int _currentShards, int _addShards)
    {
        currentShards = _currentShards;
        addShards = _addShards;
        addShards = InfoScript.bonusAmount;
        //Prepare file json ở đây. Cần điều chỉnh tùy thuộc vào mẫu JSON của Hương.

        wrapper = LoadLevelRequirements(); //load json

        if (wrapper != null)
        {
            (startDistance, startProgress, currentIndex) = CalculateHpProgressAtStart(wrapper.levelRequirements); //Lấy thông tin hiện tại của bậc máu shard, progress và target hiện tại tại thời điểm START từ json đã load.
        }

        bgMaterial = shardBarBg.GetComponent<Image>().material;

        glowImage = Glow.GetComponent<Image>();
        glowMaterial = glowImage.material;
        glowMaterial.SetFloat("_TilingX", 1f);
        glowMaterial.SetFloat("_Glow_Intensity", 0f);

        barImage = Bar.GetComponent<Image>();
        barImage.fillAmount = (float)startProgress / startDistance;

        barMaterial = barImage.material;
        barMaterial.SetFloat("_Intensity_Bar", 1f);
        barMaterial.SetFloat("_Opacity", 0.5f);

        TurnOff(Arrow, addShardAmount);

        //Set color.
        color = (startProgress < startDistance) ? notFullColor : fullColor;
        barMaterial.color = color;

        //Set so luong target
        if (targetText != null)
        {
            targetText.text = "/ " + startDistance.ToString();
        }

        //Set so luong hien tai amount
        if (amountText != null)
        {
            amountText.text = startProgress.ToString();
        }

        if (addShardAmount != null)
        {
            addShardText = addShardAmount.GetComponent<TextMeshProUGUI>();
        }

        objectToDestroy = PrepareParticle(EndParticleContainer_1, particleLocation, out endBurstParticle);
    }


    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        if (isBarSeqPlaying)
    //        {
    //            // Dang chay ma click -> ket thuc luon.
    //            barSeq.Complete(withCallbacks: true);
    //            isBarSeqPlaying = false;
    //            return;
    //        }
    //    }
    //}

    //Khóa nút Upgrade cho đến khi barSeq chạy xong


    public void PlaySimpleBarFunction() //wrapper để bật cả coroutine animation này khi shard bay vào thanh shard bar.
    {
        if (!isBarSeqPlaying)
        {
            isBarSeqPlaying = true; //Chinh bool true để không kích hoạt nhiều lần khi đang chạy

            // GameObject ObjToDestroy = objectToDestroy;

            TurnOff(Arrow);


            totalShards = currentShards + addShards; //Tính total amount khi shard chưa bay đến
            if(barMaterial != null)

                barMaterial.SetFloat("_Opacity", 1f);

            if(addShardText != null)
                addShardText.text = "+" + addShards.ToString(); //Lấy text của Bonus từ InfoScript sang addShardAmount

            AmountUpSequence();

            ScaleIconUpSequence();

            StartCoroutine(PlaySimpleBar(wrapper.levelRequirements));

        }
    }
    public IEnumerator PlaySimpleBar(LevelRequirement[] levelRequirements) //coroutine chính để chạy bar
    {
        //FOR
        for (int i = currentIndex; i <= levelRequirements.Length; i++) //currentIndex là level hiện tại, bắt đầu từ đây, tránh chạy anim cho các level trước đã qua. 
        {
            // LevelRequirement previousRequirement = i > 0 ? levelRequirements[i - 1] : null;
            LevelRequirement requirement = levelRequirements[i];
            LevelRequirement nextRequirement = i < levelRequirements.Length - 1 ? levelRequirements[i + 1] : null;

            currentDistance = nextRequirement.required - requirement.required;

            towerHpText.text = requirement.hp.ToString();

            if (i == currentIndex)
            {
                startCount = currentShards - requirement.required;
            }
            else
            {
                startCount = 0;
            }

            if (totalShards >= nextRequirement.required) //tổng lớn hơn requirement level này
            {
                fillDuration = 0.5f;
                // amountText.text = "0";
                barMaterial.color = fullColor;

                // Create a new sequence for each iteration
                barSeq = DOTween.Sequence();

                targetText.text = "/ " + currentDistance.ToString(); //Chỉnh targetText

                yield return StartCoroutine(ProcessTargetAmount(currentDistance, i));

                // Wait for the completion of the appended sequence
                yield return barSeq.WaitForCompletion();
            }
            else
            {
                currentProgressNow = totalShards - requirement.required;
                fillDuration = 2f;
                // amountText.text = "0";

                barMaterial.color = notFullColor;

                barSeq = DOTween.Sequence();

                targetText.text = "/ " + currentDistance.ToString(); //Chỉnh targetText     //------------------> cần làm dynamic

                yield return StartCoroutine(ProcessNotFullAmount(currentDistance, i));

                yield return barSeq.WaitForCompletion();

                break;
            }

            yield return 0;

        }
    }
    private IEnumerator ProcessTargetAmount(int currentDistance, int currentIndex)
    {
        barSeq.AppendCallback(() => TurnOn(Bar, Glow));   //Tên reward? Không cần? , //Bật addShardAmount (số bonus cộng vào)

        barSeq.AppendCallback(() => PlayParticle(endBurstParticle));

        barSeq.Append(glowMaterial.DOFloat(11f, "_TilingX", 0.01f)); //Tăng glow tiling 
        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 0.01f)); //Tắt glow intensity
        barSeq.Join(glowMaterial.DOFloat(Mathf.Clamp((float)currentProgressNow / currentDistance, 1f, 12f), "_TilingX", 0.01f)); //Chạy glow đến điểm fill

        barSeq.Join(barImage.DOFillAmount((float)startCount / currentDistance, 0.01f));

        barSeq.Append(barMaterial.DOFloat(1f, "_Opacity", fillDuration));  //Để opacity bar trong 1 giây
        barSeq.Join(barMaterial.DOFloat(1f, "_Intensity_Bar", fillDuration));  //Để intensity bar trong 1 giây

        barSeq.Join(glowMaterial.DOFloat(1.5f, "_Glow_Intensity", fillDuration).SetDelay(0.5f)); //Tăng glow
        barSeq.Join(Bar.transform.DOScale(new Vector3(1f, 1.1f, 1f), fillDuration / 2).SetLoops(2, LoopType.Yoyo)); //Scale bar lên xuống
        barSeq.Join(amountText.transform.DOScale(new Vector3(1f, 1.3f, 1f), fillDuration / 2).SetLoops(2, LoopType.Yoyo)); //Scale số lên xuống
        barSeq.Join(barImage.DOFillAmount(1f, fillDuration));
        barSeq.Join(amountText.DOCounter(startCount, currentDistance, fillDuration));

        barSeq.Append(barMaterial.DOFloat(0f, "_Opacity", fillDuration * 0.7f)); //Giảm opacity của bar
        barSeq.Join(barMaterial.DOFloat(0f, "_Intensity_Bar", fillDuration * 0.7f)); //Giảm intensity của bar
        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", fillDuration * 0.7f)); //Giảm intensity của glow
        barSeq.Join(shardIconGlow.GetComponent<Image>().DOFade(0f, fillDuration * 0.7f)); //Glow fade back to 0
        barSeq.AppendCallback(() => StopParticle(endBurstParticle));

        yield return null;
    }
    private IEnumerator ProcessNotFullAmount(int currentDistance, int currentIndex)
    {
        barSeq.AppendCallback(() => PlayParticle(endBurstParticle));

        barSeq.AppendCallback(() => TurnOn(Bar, Glow));   //Tên reward? Không cần? , //Bật addShardAmount (số bonus cộng vào)

        barSeq.Append(glowMaterial.DOFloat(11f, "_TilingX", 0.01f)); //Tăng glow tiling 
        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", 0.01f)); //Tắt glow intensity
        barSeq.Join(glowMaterial.DOFloat(Mathf.Clamp((float)currentProgressNow / currentDistance, 1f, 12f), "_TilingX", 0.01f)); //Chạy glow đến điểm fill
        barSeq.Join(barImage.DOFillAmount((float)startCount / currentDistance, 0.01f));

        barSeq.Append(barMaterial.DOFloat(1f, "_Opacity", fillDuration));  //Để opacity bar trong 1 giây
        barSeq.Join(barMaterial.DOFloat(1f, "_Intensity_Bar", fillDuration));  //Để intensity bar trong 1 giây
        barSeq.Join(glowMaterial.DOFloat(1.5f, "_Glow_Intensity", fillDuration).SetDelay(0.5f)); //Tăng glow
        barSeq.Join(Bar.transform.DOScale(new Vector3(1f, 1.1f, 1f), fillDuration / 2).SetLoops(2, LoopType.Yoyo)); //Scale bar lên xuống
        barSeq.Join(amountText.transform.DOScale(new Vector3(1f, 1.3f, 1f), fillDuration / 2).SetLoops(2, LoopType.Yoyo)); //Scale số lên xuống

        barSeq.Join(barImage.DOFillAmount((float)currentProgressNow / currentDistance, fillDuration));
        barSeq.Join(amountText.DOCounter(startCount, currentProgressNow, fillDuration));

        barSeq.Join(barMaterial.DOFloat(0.5f, "_Intensity_Bar", fillDuration)); //Giảm intensity của bar
        barSeq.Join(glowMaterial.DOFloat(0f, "_Glow_Intensity", fillDuration)); //Giảm intensity của glow
        barSeq.Join(shardIconGlow.GetComponent<Image>().DOFade(0f, fillDuration)); //Glow fade back to 0
        barSeq.AppendCallback(() => StopParticle(endBurstParticle));

        barSeq.OnComplete(() =>
        {
            // Chinh bool lai thanh false
            if (isBarSeqPlaying)
            {
                isBarSeqPlaying = false;
            }

            InvokeBonusFlyDoneWithDefault();
        });

        yield return null;
    }
    private (int targetDistance, int currentProgress, int currentIndex) CalculateHpProgressAtStart(LevelRequirement[] levelRequirements)
    {
        if (levelRequirements != null && levelRequirements.Length > 0)
        {
            // Initialize variables for target points and current progress
            int targetDistance = 0;
            int currentProgress = 0;
            int currentIndex = 0;

            // Loop through each level requirement
            for (int i = 0; i < levelRequirements.Length; i++)
            {
                currentIndex = i; //currentIndex là level hiện tại, bắt đầu từ đây, tránh chạy anim cho các level trước đã qua.

                // LevelRequirement previousRequirement = i > 0 ? levelRequirements[i - 1] : null;
                LevelRequirement requirement = levelRequirements[i];
                LevelRequirement nextRequirement = i < levelRequirements.Length - 1 ? levelRequirements[i + 1] : null;

                towerHpText.text = requirement.hp.ToString();

                // Check if current points surpass the required points for the current level
                if (currentShards >= requirement.required && currentShards < nextRequirement.required)
                {
                    // Calculate current progress based on the difference between current points and required points
                    currentProgress = currentShards - requirement.required;

                    // Check if there's another level requirement
                    if (i < levelRequirements.Length - 1)
                    {
                        // Calculate target points based on the difference between the required points for the next level and the current level
                        targetDistance = nextRequirement.required - requirement.required;
                    }
                    else
                    {
                        // If it's the last level, set target points to 0
                        targetDistance = 0;
                    }

                    break; // Break out of the loop since the current level is found
                }
            }

            return (targetDistance, currentProgress, currentIndex);
        }

        return (0, 0, 0);
    }
    public void InvokeBonusFlyDoneWithDefault()
    {
        BonusFlyDone?.Invoke(InfoScript.LevelVar); // Change defaultFakeInt to the desired default or predefined value
    }
    private void ScaleIconUpSequence()
    {
        ScaleIcon = DOTween.Sequence();
        ScaleIcon.Append(bgMaterial.DOFloat(0f, "_Intensity", 0.01f)); //Reset Intensity of background    1
        ScaleIcon.Join(shardIconGlow.GetComponent<Image>().DOFade(0.0f, 0.01f)); //Glow xung quanh shard to 0    1
        ScaleIcon.Append(shardIcon.DOScale(new Vector3(1.5f, 1.5f, 0), 0.25f) //Scale  icon shard, đoạn này chỉ nên 1 lần     1
            .SetEase(Ease.InSine)
            .SetLoops(4, LoopType.Yoyo));

        ScaleIcon.Join(shardIconGlow.GetComponent<Image>().DOFade(1.0f, 0.5f) //Start glow quanh shard, đoạn này chỉ nên 1 lần     1
            .SetLoops(2, LoopType.Yoyo));

        ScaleIcon.Join(bgMaterial.DOFloat(1.0f, "_Intensity", 0.5f) //Background flash, đoạn này chỉ nên 1 lần    1
            .SetLoops(2, LoopType.Yoyo));
    }
    private void AmountUpSequence()
    {
        shardNumberSeq = DOTween.Sequence();

        shardNumberSeq.AppendInterval(0.5f);

        shardNumberSeq.AppendCallback(() => TurnOn(addShardAmount));

        shardNumberSeq.Append(addShardText.DOFade(1f, 0.3f)); //Fade in addShardAmount
        shardNumberSeq.Join(addShardAmount.transform.DOLocalMove(new Vector3(0f, 5f, 0f), 1f).SetEase(Ease.OutSine).SetRelative(true)); //Di chuyển addShardAmount --> chỉnh đoạn này
        shardNumberSeq.Join(addShardAmount.transform.DOScale(new Vector3(1f, 1.2f, 0f), 0.5f).SetLoops(2, LoopType.Yoyo)); //Scale addAmount

        shardNumberSeq.AppendInterval(3f);

        shardNumberSeq.Join(addShardText.DOFade(0f, 1.5f));
        shardNumberSeq.Append(addShardAmount.transform.DOLocalMove(new Vector3(0f, -5f, 0f), 1f).SetRelative(true));
        shardNumberSeq.AppendCallback(() => TurnOff(addShardAmount));
    }
    void PlayParticle(ParticleSystem particleSys)
    {
        if (particleSys != null)
        {
            particleSys.Play();
        }
    }
    void StopParticle(ParticleSystem particleSys)
    {

        if (particleSys != null)
        {
            particleSys.Stop();
        }
    }
    private void TurnOn(params GameObject[] gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }
    }
    private void TurnOff(params GameObject[] gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
    }
    GameObject PrepareParticle(GameObject particleContainer, Transform startTransform, out ParticleSystem particleSys)
    {
        particleSys = null;

        GameObject destroyContainer = Instantiate(particleContainer, startTransform.parent.parent);
        destroyContainer.transform.position = startTransform.position;
        destroyContainer.transform.rotation = startTransform.rotation;

        if (destroyContainer != null)
        {
            particleSys = destroyContainer.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        }

        return destroyContainer;
    }
    // private void PlayBurstParticleSystem(ParticleSystem sys, float delayTime, float particleAmount, int cycles, float waitBetweenCycles)
    // {
    //     if (sys != null)
    //     {
    //         ParticleSystem.Burst[] bursts = { new ParticleSystem.Burst(delayTime, (short)particleAmount, (short)particleAmount, (short)cycles, waitBetweenCycles) };
    //         sys.emission.SetBursts(bursts);
    //         sys.Play();
    //     }
    //     else
    //     {
    //         Debug.LogError("Particle System is not assigned.");
    //     }
    // }
    private void DestroyObject(GameObject gameObject)
    {
        Destroy(gameObject); // Destroy the game object
    }
    void OnDestroy()
    {
        DestroyObject(objectToDestroy);
    }
    private LevelRequirementsWrapper LoadLevelRequirements()
    {
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            return JsonUtility.FromJson<LevelRequirementsWrapper>(jsonText);
        }

        return null;
    }
}