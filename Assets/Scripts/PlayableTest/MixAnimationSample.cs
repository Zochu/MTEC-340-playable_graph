using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations; //Import needed namespace
using UnityEngine.Audio;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]

public class MixAnimationSample : MonoBehaviour
{
    PlayableGraph graph;

    AnimationClipPlayable clipPlayable1, clipPlayable2;
    AnimationMixerPlayable animMixer;
    AnimationPlayableOutput animOutput;

    [SerializeField] AnimationClip animClip1, animClip2;
    //AnimationCotrollers can also be used in PlayableGraph

    AudioClipPlayable audioPlayable1, audioPlayable2;
    AudioMixerPlayable audioMixer;
    AudioPlayableOutput audioOutput;

    [SerializeField] AudioClip audioClip1, audioClip2;

    [Range(0f, 1f)]
    [SerializeField] float weight;

    private void Start()
    {
        //Create a PlayableGraph when starting
        graph = PlayableGraph.Create();

        //Set the playback speed of the PlayableGraph to GameTime
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        //Create an AnimationMixer with 2 Inputs
        animMixer = AnimationMixerPlayable.Create(graph, 2);

        /*
        Alternative syntax
        
        Use follwing codes when Inputs number is not sure when creating the mixer

        animMixer = AnimationMixerPlayable.Create(graph);
        animMixer.SetInputCount(2);
        */



        //Create two AnimationPlayables, and set the AnimationClips to them
        clipPlayable1 = AnimationClipPlayable.Create(graph, animClip1);
        clipPlayable2 = AnimationClipPlayable.Create(graph, animClip2);


        //Connect two Playbles to the Mixer's 1st and 2nd Input port 
        graph.Connect(clipPlayable1, 0, animMixer, 0);
        graph.Connect(clipPlayable2, 0, animMixer, 1);

        //Set the Weight of each Input
        //Note that usually the sum of all ImputWeight should be 1
        animMixer.SetInputWeight(0, 1);
        animMixer.SetInputWeight(1, 0);

        /*
        Alternative syntax

        Use following code could set the weight of a Playable when connecting it to the Mixer
        When using AddInput(), it creates a new Input for the Mixer automaticly

        animMixer.AddInput(clipPlayable1, 0, 1f);
        animMixer.AddInput(clipPlayable2, 0, 0f);
        */




        //Create an Output and link it to Animator
        animOutput = AnimationPlayableOutput.Create(graph, "AnimOut", GetComponent<Animator>());


        //Link the AnimationMixer to the Output
        animOutput.SetSourcePlayable(animMixer);




        //Do same thing to AudioPlayables
        audioMixer = AudioMixerPlayable.Create(graph, 2);

        audioPlayable1 = AudioClipPlayable.Create(graph, audioClip1, true); //3rd argument, the boolean controls looping
        audioPlayable2 = AudioClipPlayable.Create(graph, audioClip2, true);

        graph.Connect(audioPlayable1, 0, audioMixer, 0);
        graph.Connect(audioPlayable2, 0, audioMixer, 1);

        audioMixer.SetInputWeight(0, 1f);
        audioMixer.SetInputWeight(1, 0f);

        audioOutput = AudioPlayableOutput.Create(graph, "AnimOut", GetComponent<AudioSource>());

        audioOutput.SetSourcePlayable(audioMixer);




        //Play the PlayableGraph
        graph.Play();
    }

    private void Update()
    {
        //Set InputWeight of two Playeables at runtime, create a smooth linear transition
        animMixer.SetInputWeight(0, 1 - weight);
        animMixer.SetInputWeight(1, weight);

        audioMixer.SetInputWeight(0, 1 - weight);
        audioMixer.SetInputWeight(1, weight);
    }

    private void OnDisable()
    {
        //Distroy the PlayableGraph when the gameObject is distroied
        graph.Destroy();
    }
}
