using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


namespace BezierSolution
{
	[AddComponentMenu( "Bezier Solution/Bezier Walker With Speed" )]
	public class BezierWalkerWithSpeed : BezierWalker
	{
		//public List<BezierSpline> bezierRailList = new List<BezierSpline>();

		public BezierSpline spline; 
		public int splineSelected = 0;
		
		public GameObject railWay;

		public GameObject railPrefab;
		public GameObject railPrefabB;

		public GameObject emptyPrefab;

		public TravelMode travelMode;
		private float speed = 0f;
		public float speedInput;
		private float speedTorq = 30f;

		[SerializeField]
		[Range( 0f, 1f )]
		private float m_normalizedT = 0f;

		public override BezierSpline Spline { get { return spline; } }

		public override float NormalizedT
		{
			get { return m_normalizedT; }
			set { m_normalizedT = value; }
		}

		public float movementLerpModifier = 10f;
		public float rotationLerpModifier = 10f;

		public LookAtMode lookAt = LookAtMode.Forward;

		private bool isGoingForward = true;
		public override bool MovingForward { get { return ( speed > 0f ) == isGoingForward; } }

		public UnityEvent onPathCompleted = new UnityEvent();
		private bool onPathCompletedCalledAt1 = false;
        private bool onPathCompletedCalledAt0 = false;

		private int railsCount = 10;

		public GameObject currentRailA;
		public GameObject currentRailB;

		public int tileRun = 0;

		public void Spawn(int i, GameObject railPrefabA, GameObject railPrefabB)
		{
			Rail.currentA = Instantiate(railPrefabA, Rail.currentA.transform.GetChild(0).position, Quaternion.identity);

            Rail.currentA.transform.parent = railWay.transform;
            BezierSpline tempBezierRailA = Rail.currentA.GetComponentsInChildren<BezierSpline>()[0];
			List<Rail.Tile> localTileA = new List<Rail.Tile>();
			localTileA.Insert(0, new Rail.Tile() { type = "A", bezierSpline = tempBezierRailA });
			Rail.segment.Insert(i, new Rail.Segment() { tile = localTileA });

            if (Rail.duo)
            {
				Rail.currentB = Instantiate(railPrefabB, Rail.currentB.transform.GetChild(0).position, Quaternion.identity);
            }
            else
            {
				Rail.currentB = Instantiate(railPrefabB, Rail.currentA.transform.GetChild(0).position, Quaternion.identity);
                Rail.duo = true;
            }

            Rail.currentB.transform.parent = railWay.transform;
            BezierSpline tempCurrentRailB = Rail.currentB.GetComponentsInChildren<BezierSpline>()[0];
			List<Rail.Tile> localTileB = new List<Rail.Tile>();
			localTileB.Insert(1, new Rail.Tile() { type = "B", bezierSpline = tempCurrentRailB });
			Rail.segment.Insert(i, new Rail.Segment() { tile = localTileB });
		}

		private void Start()
        {
			Rail.currentA = railPrefab;
			Rail.currentB = emptyPrefab;

			for (int i = 0; i < railsCount; i++)
            {
				Spawn(i, railPrefab, emptyPrefab);
            }
        }

		

		private void Update()
		{
			speedInput = Input.GetAxis("Vertical");

			Execute( Time.deltaTime);
		}

		public override void Execute( float deltaTime)
		{
            speed = speedInput * speedTorq;
			float targetSpeed = ( isGoingForward ) ? speed : -speed;
			splineSelected = System.Math.Abs(splineSelected);
            Vector3 targetPos = Rail.GetRail(splineSelected, tileRun).MoveAlongSpline( ref m_normalizedT, targetSpeed * deltaTime );
			transform.position = targetPos;

			bool movingForward = MovingForward;

			if( lookAt == LookAtMode.Forward )
			{
				BezierSpline.Segment segment = spline.GetSegmentAt( m_normalizedT );
				Quaternion targetRotation;
				if( movingForward )
					targetRotation = Quaternion.LookRotation( segment.GetTangent(), segment.GetNormal() );
				else
					targetRotation = Quaternion.LookRotation( -segment.GetTangent(), segment.GetNormal() );

				transform.rotation = Quaternion.Lerp( transform.rotation, targetRotation, rotationLerpModifier * deltaTime );
			}
			else if( lookAt == LookAtMode.SplineExtraData )
				transform.rotation = Quaternion.Lerp( transform.rotation, spline.GetExtraData( m_normalizedT, extraDataLerpAsQuaternionFunction ), rotationLerpModifier * deltaTime );

			if( movingForward )
			{
				if( m_normalizedT >= 1f )
				{
					if( travelMode == TravelMode.Once )
						m_normalizedT = 1f;
					else if( travelMode == TravelMode.Loop)
                    {
						m_normalizedT -= 1f;
						splineSelected = splineSelected + 1;
					}
					else
					{
						m_normalizedT = 2f - m_normalizedT;
						isGoingForward = !isGoingForward;
					}

					if( !onPathCompletedCalledAt1 )
					{
						onPathCompletedCalledAt1 = true;
						
#if UNITY_EDITOR
						if ( UnityEditor.EditorApplication.isPlaying )
#endif
							onPathCompleted.Invoke();
					}

					
				}
				else
				{
					onPathCompletedCalledAt1 = false;
				}
			}
			else
			{
				if( m_normalizedT <= 0f )
				{
					if( travelMode == TravelMode.Once )
						m_normalizedT = 0f;
					else if( travelMode == TravelMode.Loop )
                    {
						m_normalizedT += 1f;
						splineSelected = splineSelected - 1;
					}
					else
					{
						m_normalizedT = -m_normalizedT;
						isGoingForward = !isGoingForward;
					}

					if( !onPathCompletedCalledAt0 )
					{
						onPathCompletedCalledAt0 = true;
#if UNITY_EDITOR
						if ( UnityEditor.EditorApplication.isPlaying )
#endif
							onPathCompleted.Invoke();
					}
				}
				else
				{
					onPathCompletedCalledAt0 = false;
				}
			}
		}
	}
}