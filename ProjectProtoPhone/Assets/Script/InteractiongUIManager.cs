using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

enum Dir
{
   Forward,
   Right,
   left,
   back
   
}

 [ExecuteAlways]
public class InteractiongUIManager : MonoBehaviour
{
   [SerializeField] private Transform target;
   [SerializeField] private Dir arcDir;
   private const float ArcAngle = 180;
   [SerializeField] private float arcRadius;
   [SerializeField, Range(0,8)] private int numberOfBubbles;
   private int _numberOfDisplayedBubbles;
   private float _angleBetweenBubbles;
   [SerializeField, Range(0.1f,2)] private float bubbleSize;
   [SerializeField] private bool showGizmos;

   [SerializeField] private List<Image> _imagesBuules = new(8);
   
   [Button]
   public void UpdateUI()
   {
      if (target == null) return;
      foreach (var image in _imagesBuules) image.gameObject.SetActive(false);

      Vector3 arcDir = this.arcDir switch
      {
         Dir.Forward => Vector3.left,
         Dir.Right => Vector3.forward,
         Dir.left => Vector3.back,
         Dir.back => Vector3.right,
         _ => Vector3.zero
      };
         
      _angleBetweenBubbles = ArcAngle / (numberOfBubbles +1) +0.1f;

      _numberOfDisplayedBubbles = 0;
         
      for (float i = _angleBetweenBubbles; i <= ArcAngle; i+= _angleBetweenBubbles)
      {
         if (_numberOfDisplayedBubbles == numberOfBubbles) break;   
            
         var dir = Quaternion.Euler(0, i, 0) * arcDir * arcRadius;
         _imagesBuules[_numberOfDisplayedBubbles].gameObject.SetActive(true);
         _imagesBuules[_numberOfDisplayedBubbles].transform.position = target.position + dir;
         _numberOfDisplayedBubbles++;
      }
   }
   
#if UNITY_EDITOR
   private void OnDrawGizmos()
   {      
      if (showGizmos)
      {   
         if (target == null) return;
         
         Handles.color = Color.blue;

         Vector3 arcDir = this.arcDir switch
         {
            Dir.Forward => Vector3.left,
            Dir.Right => Vector3.forward,
            Dir.left => Vector3.back,
            Dir.back => Vector3.right,
            _ => Vector3.zero
         };
         
         Handles.DrawWireArc(target.position, -Camera.main.transform.forward, arcDir, ArcAngle, arcRadius, 2.5f);
         
         _angleBetweenBubbles = ArcAngle / (numberOfBubbles +1) +0.1f;

         _numberOfDisplayedBubbles = 0;
         
         for (float i = _angleBetweenBubbles; i <= ArcAngle; i+= _angleBetweenBubbles)
         {
            if (_numberOfDisplayedBubbles == numberOfBubbles) break;   
            
            var dir = Quaternion.Euler(0, i, 0) * arcDir * arcRadius;
            Debug.DrawRay(target.position, dir, Color.green);
            Handles.color = Color.yellow;
            Handles.DrawSolidDisc(target.position + dir, -Camera.main.transform.forward, bubbleSize);
            _numberOfDisplayedBubbles++;
         }
      }
   }
#endif
}