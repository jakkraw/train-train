﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class World : MonoBehaviour {

    public Train train;
    public Environment environment;
    float _newStationDistance = 100;

    private void Start()
    {
        foreach(var seat in train.seats)
            if (Random.value < .4) seat.Place(Passenger.SpawnRandom());
    }

    void Update () {
        SpawnStations(); 
        HandleInput();
        MoveWorld();
        train.Decelerate();

        var rect = ClosestStation().GetComponent<BoxCollider2D>().bounds;
        var enablePassengerMove = train.Speed == 0 && train.seats.All(seat => rect.Contains(seat.transform.position));

        foreach (Seat seat in FindObjectsOfType<Seat>())
        {
            var collider = seat.GetComponent<BoxCollider>();
            collider.enabled = enablePassengerMove;
        }

        foreach (Passenger passenger in FindObjectsOfType<Passenger>())
        {
            var image = passenger.GetComponent<Image>();
            var color = image.color;
            color.a = enablePassengerMove ? 1f : 0.5f;
            image.color = color;
        }

        var timage = train.GetComponent<SpriteRenderer>();
        var tcolor = timage.color;
        tcolor.a = enablePassengerMove ? 0.5f : 1f;
        timage.color = tcolor;

    }

    void MoveWorld()
    {
        environment.SetMoveSpeed(-train.Speed);
        foreach (GameObject station in GameObject.FindGameObjectsWithTag("Station"))
        {
            station.transform.Translate(Time.deltaTime * -train.Speed, 0.0f, 0.0f);
        }
    }

    void SpawnStations()
    {
        var trainPos = train.transform.position;
        var stationPos = ClosestStation().transform.position;
        if (Vector3.Distance(trainPos, stationPos) > _newStationDistance && trainPos.x > stationPos.x)
        {
            Destroy(ClosestStation().gameObject);
            StationSpawner.Spawn();
            _newStationDistance = Random.Range(30, 130);
        }


    }

    void SwapSeat(Seat a, Seat b)
    {
        if (!a || !b) return;
        var p1 = a.Remove();
        var p2 = b.Remove();
        if(p2) a.Place(p2);
        if(p1) b.Place(p1);
    }

    Station ClosestStation()
    {
        var a = GameObject.FindGameObjectsWithTag("Station");
        return a[0].GetComponent<Station>();
    }

    void HandleInput()
    {
        if (Input.touchCount < 1) return;

        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.touches[0].position), out hit, 500)) {
            train.Break();
            return;
        }

        var gameObject = hit.collider.gameObject;


        if (gameObject.CompareTag("Accelerate")) {
            train.Accelerate();
        }

        if (gameObject.CompareTag("Train Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            if(!seat.isEmpty()) SwapSeat(ClosestStation().FreeSeat(), seat);
        }

        if (gameObject.CompareTag("Station Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            var train_seat = train.FreeSeat();
            if(train_seat) { SwapSeat(train_seat, seat); }
        }

    }

}
