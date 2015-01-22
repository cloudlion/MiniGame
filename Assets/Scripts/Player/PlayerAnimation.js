#pragma strict

class MoveAnimation {
	// The animation clip
	var clip : AnimationClip;
	
	// The velocity of the walk or run cycle in this clip
	var velocity : Vector3;
	
	// Store the current weight of this animation
	@HideInInspector
	public var weight : float;
	
	// Keep track of whether this animation is currently the best match
	@HideInInspector
	public var currentBest : boolean = false;
	
	// The speed and angle is directly derived from the velocity,
	// but since it's slightly expensive to calculate them
	// we do it once in the beginning instead of in every frame.
	@HideInInspector
	public var speed : float;
	@HideInInspector
	public var angle : float;
	
	public function Init () {
		velocity.y = 0;
		speed = velocity.magnitude;
		angle = PlayerAnimation.HorizontalAngle (velocity);
	}
}

var rigid : Rigidbody;
var rootBone : Transform;
var upperBodyBone : Transform;
var maxIdleSpeed : float = 0.5;
var minWalkSpeed : float = 2.0;
var idle : AnimationClip;
var turn : AnimationClip;
var shootAdditive : AnimationClip;
var moveAnimations : MoveAnimation[];


private var tr : Transform;
private var lastPosition : Vector3 = Vector3.zero;
private var velocity : Vector3 = Vector3.zero;
private var localVelocity : Vector3 = Vector3.zero;
private var speed : float = 0;
private var angle : float = 0;
private var lowerBodyDeltaAngle : float = 0;
private var idleWeight : float = 0;
private var lowerBodyForwardTarget : Vector3 = Vector3.forward;
private var lowerBodyForward : Vector3 = Vector3.forward;
private var bestAnimation : MoveAnimation = null;
private var lastFootstepTime : float = 0;
private var lastAnimTime : float = 0;

public var animationComponent : Animation;

function Awake () {
	tr = rigid.transform;
	lastPosition = tr.position;
	
	for (var moveAnimation : MoveAnimation in moveAnimations) {
		moveAnimation.Init ();
		animationComponent[moveAnimation.clip.name].layer = 1;
		animationComponent[moveAnimation.clip.name].enabled = true;
	}
	animationComponent.SyncLayer (1);
	
	animationComponent[idle.name].layer = 2;
	animationComponent[turn.name].layer = 3;
	animationComponent[idle.name].enabled = true;
	
	animationComponent[shootAdditive.name].layer = 4;
	animationComponent[shootAdditive.name].weight = 1;
	animationComponent[shootAdditive.name].speed = 0.6;
	animationComponent[shootAdditive.name].blendMode = AnimationBlendMode.Additive;
	
	//animation[turn.name].enabled = true;
}

function OnStartFire () {
	if (Time.timeScale == 0)
		return;
	
	animationComponent[shootAdditive.name].enabled = true;
}

function OnStopFire () {
	animationComponent[shootAdditive.name].enabled = false;
}

function FixedUpdate () {
	velocity = (tr.position - lastPosition) / Time.deltaTime;
	localVelocity = tr.InverseTransformDirection (velocity);
	localVelocity.y = 0;
	speed = localVelocity.magnitude;
	angle = HorizontalAngle (localVelocity);
	
	lastPosition = tr.position;
}

function Update () {
	idleWeight = Mathf.Lerp (idleWeight, Mathf.InverseLerp (minWalkSpeed, maxIdleSpeed, speed), Time.deltaTime * 10);
	animationComponent[idle.name].weight = idleWeight;
	
	if (speed > 0) {
		var smallestDiff : float = Mathf.Infinity;
		for (var moveAnimation : MoveAnimation in moveAnimations) {
			var angleDiff : float = Mathf.Abs(Mathf.DeltaAngle (angle, moveAnimation.angle));
			var speedDiff : float = Mathf.Abs (speed - moveAnimation.speed);
			var diff : float = angleDiff + speedDiff;
			if (moveAnimation == bestAnimation)
				diff *= 0.9;
			
			if (diff < smallestDiff) {
				bestAnimation = moveAnimation;
				smallestDiff = diff;
			}
		}
		
		animationComponent.CrossFade (bestAnimation.clip.name);
	}
	else {
		bestAnimation = null;
	}
	
	if (lowerBodyForward != lowerBodyForwardTarget && idleWeight >= 0.9)
		animationComponent.CrossFade (turn.name, 0.05);
	
	if (bestAnimation && idleWeight < 0.9) {
		var newAnimTime = Mathf.Repeat (animationComponent[bestAnimation.clip.name].normalizedTime * 2 + 0.1, 1);
		if (newAnimTime < lastAnimTime) {
			if (Time.time > lastFootstepTime + 0.1) {
				lastFootstepTime = Time.time;
			}
		}
		lastAnimTime = newAnimTime;
	}
}

static function HorizontalAngle (direction : Vector3) {
	return Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
}
