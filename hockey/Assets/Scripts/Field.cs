using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Team
{
    public Transform side;
    public Goal goal;
    public StrikerAgent striker;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI scoreText;

    [HideInInspector] public int score = 0;
}

public class Field : MonoBehaviour
{
    public Puck puck;
    public Team rightTeam;
    public Team leftTeam;

    // Start is called before the first frame update
    void Start()
    {
        rightTeam.striker.team = rightTeam;
        rightTeam.goal.team = rightTeam;

        leftTeam.striker.team = leftTeam;
        leftTeam.goal.team = leftTeam;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!puck.HasServed)
            puck.Oscillate(20.0f, 6.0f);

        rightTeam.rewardText.text = rightTeam.striker.GetCumulativeReward().ToString("0.00");
        leftTeam.rewardText.text = leftTeam.striker.GetCumulativeReward().ToString("0.00");

        rightTeam.scoreText.text = rightTeam.score.ToString("000");
        leftTeam.scoreText.text = leftTeam.score.ToString("000");
    }

    void PlacePuck (StrikerAgent striker)
    {
        puck.transform.position = striker.transform.position + striker.transform.forward * 1.0f;
        puck.rb.velocity = Vector3.zero;
    }

    void PlaceStrikers ()
    {
        rightTeam.striker.transform.position = rightTeam.striker.startPos;
        rightTeam.striker.rb.velocity = Vector3.zero;

        leftTeam.striker.transform.position = leftTeam.striker.startPos;
        rightTeam.striker.rb.velocity = Vector3.zero;
    }

    public void ResetField (StrikerAgent strikerWServe)
    {
        PlaceStrikers();
        PlacePuck(strikerWServe);
    }

    public void ScoreGoal (Team team)
    {
        Team otherTeam = team == rightTeam ? leftTeam : rightTeam;

        otherTeam.score++;
        otherTeam.striker.AddReward(5.0f);

        team.striker.AddReward(-1.0f);

        team.striker.EndEpisode();
        otherTeam.striker.EndEpisode();
    }
}
