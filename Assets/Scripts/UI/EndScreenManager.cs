using UnityEngine;
using UnityEngine.InputSystem;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] GameObject earth;
    [SerializeField] GameObject mars;
    [SerializeField] GameObject moon;

    [SerializeField] MockPlayer terry;
    [SerializeField] MockPlayer spencer;
    [SerializeField] MockPlayer paris;
    [SerializeField] MockPlayer andrew;

    [SerializeField] Transform[] landPositions;
    [SerializeField] Transform[] spacePositions;

    [SerializeField] private GameObject Meteor;
    
    [SerializeField] public bool StartInLandingPos = true;
    
    private void Start()
    {
        VoteEffectManager.Instance.ApplyStoredEffects(-1, -1);

        switch (VoteEffectManager.Instance.Ending)
        {
            case Ending.Mars:
                earth.SetActive(false);
                moon.SetActive(false);
                mars.SetActive(true);
                Meteor.SetActive(false);
                StartInLandingPos = true;
                break;
            
            case Ending.Moon:
                earth.SetActive(true);
                moon.SetActive(true);
                mars.SetActive(false);
                StartInLandingPos = true;
                Meteor.SetActive(false);
                break;
            
            case Ending.Space:
                earth.SetActive(true);
                moon.SetActive(false);
                mars.SetActive(false);
                StartInLandingPos = false;
                Meteor.SetActive(true);
                break;
        }

        SetStartPos();
    
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
            switch(player.CharacterType)
            {
                case CharacterType.Triceratops:
                    terry.gameObject.SetActive(true);
                    break;
                case CharacterType.Parasaurolophus:
                    paris.gameObject.SetActive(true);
                    break;
                case CharacterType.Spinosaurus: 
                    spencer.gameObject.SetActive(true);
                    break;
                case CharacterType.Therizinosaurus:
                    andrew.gameObject.SetActive(true);
                    break;
            }
        }
    }
    
    private void SetStartPos()
    {
        if (StartInLandingPos)
        {
            terry.transform.position = landPositions[0].position;
            spencer.transform.position = landPositions[1].position;
            paris.transform.position = landPositions[2].position;
            andrew.transform.position = landPositions[3].position;
            
            terry.transform.rotation = landPositions[0].rotation;
            spencer.transform.rotation = landPositions[1].rotation;
            paris.transform.rotation = landPositions[2].rotation;
            andrew.transform.rotation = landPositions[3].rotation;
        }
        else
        {
            terry.transform.position = spacePositions[0].position;
            spencer.transform.position = spacePositions[1].position;
            paris.transform.position = spacePositions[2].position;
            andrew.transform.position = spacePositions[3].position;
        }
    }
}
