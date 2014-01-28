#pragma strict
///////////////////////////////////////
//// menuScript created by patrick rasmussen 
//// Main menu script handles the selections through the games menu system.
///////////////////////////////////////

var yourCursor : Texture2D;  // Your cursor texture
var cursorSizeX : int = 16;  // Your cursor size x
var cursorSizeY : int = 16;  // Your cursor size y
var mouseX:float = 864.0f;
var mouseY:float = 256.0f; 
var iconSpeed:float = 3.0f;
function Awake()
{
    Screen.showCursor = false;
}
 
function OnGUI()
{
    GUI.DrawTexture (Rect(mouseX -cursorSizeX/2, mouseY -cursorSizeY/2, cursorSizeX, cursorSizeY), yourCursor);
    
}
//// updates to see if raycast returns a hit and checks that hit name for advancement or exit.
function Update()
{
	if(Input.GetAxis("Horizontal") > 0.6f  || Input.GetAxis("Horizontal") < -0.6f)
	{
		mouseX += Input.GetAxis("Horizontal")* iconSpeed;
	}
	
	
	if(Input.GetAxis("Vertical")  > 0.6f  || Input.GetAxis("Vertical") < -0.6f )
	{
		 	 mouseY += Input.GetAxis("Vertical")*iconSpeed;
    }
    
    if (mouseX < 600)
    {
        mouseX = 600;
    }
    //check if the left mouse has been pressed down this frame
    if (Input.GetButtonDown("Trick2"))
    {
        //empty RaycastHit object which raycast puts the hit details into
        var hit : RaycastHit;
        //ray shooting out of the camera from where the mouse is
        var ray : Ray = Camera.main.ScreenPointToRay(Vector3(mouseX, mouseY,0));
  		print("fired a ray" + mouseX + mouseY);
 		// checks the hit if so tells us the name in debug and then checks what to do with that name.
        if (Physics.Raycast(ray, hit))
        {
       
        // starts game.
        if ( hit.collider.name == "startCube")
         {
         	Application.LoadLevel(2);
         }
         // loads controller guide
         if (hit.collider.name == "howToCube")
         {
         	Application.LoadLevel(1);
         } 
         // loads how to pla
         if (hit.collider.name == "exitCube")
         {
         	 Application.Quit();
         } 
       
        }
    }
}
