using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations; //Import needed namespace
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]


public class PlayanimationSample : MonoBehaviour
{
    PlayableGraph graph;
    AnimationClipPlayable clipPlayable;
    AnimationPlayableOutput output;

    [SerializeField] AnimationClip animClip1;

    private void Start()
    {
        //Create a PlayableGraph when starting–
        graph = PlayableGraph.Create();

        //Set the playback speed of the PlayableGraph to GameTime
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        //Create an AnimationPlayable, and set the AnimationClip to it
        clipPlayable = AnimationClipPlayable.Create(graph, animClip1);

        //Create an Output and link it to Animator
        output = AnimationPlayableOutput.Create(graph, "AnimOut", GetComponent<Animator>());

        //Link the AnimationPlayable to the Output
        output.SetSourcePlayable(clipPlayable);

        //Play the PlayableGraph
        graph.Play();
    }

    private void Update()
    {
        //When pressing P, pause the animation if it's playing, and play the animation from start if it's paused
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (clipPlayable.GetPlayState() == PlayState.Playing) clipPlayable.Pause();
            else
            {
                clipPlayable.SetTime(0f);
                clipPlayable.Play();
            }
        }
    }

    private void OnDisable()
    {
        //Distroy the PlayableGraph when the gameObject is distroied
        graph.Destroy();
    }
}
