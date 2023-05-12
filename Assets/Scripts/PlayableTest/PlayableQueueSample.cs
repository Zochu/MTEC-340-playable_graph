using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations; //Import needed namespace
using UnityEngine.Playables;


public class PlayableQueue : PlayableBehaviour //Creating another class inheriting PlayableBehaviour, making a custom playable
{
    AnimationMixerPlayable animMixer;

    float timeBetween;
    int currentClipIndex;

    public void Initialize(PlayableGraph graph, Playable playable, AnimationClip[] clips) //Initializing the custom playable
    {
        playable.SetInputCount(1);

        animMixer = AnimationMixerPlayable.Create(graph);

        //Connect all AnimationClips to the Mixer
        for (int i = 0; i < clips.Length; i++) animMixer.AddInput(AnimationClipPlayable.Create(graph, clips[i]), 0, 0f);

        //Set first AnimationClips as initial clip
        animMixer.SetInputWeight(0, 1f);

        graph.Connect(animMixer, 0, playable, 0);

        timeBetween = clips[0].length;
    }

    public override void PrepareFrame(Playable playable, FrameData info) //This function is called once per frame before Update
    {
        if (animMixer.GetInputCount() == 0) return;

        //Count down clip time
        timeBetween -= info.deltaTime;

        //Let the Mixer play next clip from beginning when current clip is finished
        if(timeBetween <= 0)
        {
            currentClipIndex++;
            if (currentClipIndex >= animMixer.GetInputCount()) currentClipIndex = 0;
            var currentClip = (AnimationClipPlayable)animMixer.GetInput(currentClipIndex);
            currentClip.SetTime(0f);
            timeBetween = currentClip.GetAnimationClip().length;
        }

        // Adjust the weight of the inputs
        for (int index = 0; index < animMixer.GetInputCount(); index++)
        {
            if (index == currentClipIndex) animMixer.SetInputWeight(index, 1.0f);
            else animMixer.SetInputWeight(index, 0.0f);
        }
    }

}



[RequireComponent(typeof(Animator))]
public class PlayableQueueSample : MonoBehaviour
{
    PlayableGraph graph;

    public AnimationClip[] clips;

    private void Start()
    {
        graph = PlayableGraph.Create();
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var queuePlayable = ScriptPlayable<PlayableQueue>.Create(graph); //Creating a ScriptPlayable base on the class above
        var queue = queuePlayable.GetBehaviour();

        queue.Initialize(graph, queuePlayable, clips);

        var output = AnimationPlayableOutput.Create(graph, "AnimOutput", GetComponent<Animator>());
        output.SetSourcePlayable(queuePlayable);


        graph.Play();
    }

    private void OnDisable()
    {
        graph.Destroy();
    }
}
