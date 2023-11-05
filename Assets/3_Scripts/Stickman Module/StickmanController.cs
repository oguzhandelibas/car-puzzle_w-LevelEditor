using System.Collections.Generic;
using CarLotJam.CarModule;
using CarLotJam.ClickModule;
using CarLotJam.GridModule;
using ODProjects.LevelEditor;
using CarLotJam.Pathfind;
using DG.Tweening;
using UnityEngine;

namespace CarLotJam.StickmanModule
{
    public class StickmanController : MonoBehaviour, IClickable, IElement
    {
        #region FIELDS

        [SerializeField] private StickmanAnimationController _stickmanAnimationController;
        [SerializeField] private EmotionController emotionController;
        [SerializeField] private GameObject outlineObject;
        [SerializeField] private ColorData colorData;
        [SerializeField] private ColorSetter colorSetter;

        public SelectedColor selectedColor;
        public List<Point> targetPath;
        private Point _stickmanPoint;
        private Point _targetPoint;

        private CarController _carController;

        #endregion

        #region VARIABLES

        private bool _onHold;
        private bool _onMove;

        #endregion

        #region PROPERTIES

        public bool IsHold
        {
            get { return _onHold; }
            set
            {
                if (!_onMove)
                {
                    if (value) Hold();
                    else Release();
                    _onHold = value;
                }

            }
        }

        #endregion

        #region STICKMAN FUNCTIONS

        public void InitializeElement(SelectedDirection selectedDirection, SelectedColor selectedColor, Point elementPoint)
        {
            _stickmanPoint = elementPoint;
            this.selectedColor = selectedColor;
            colorSetter.SetMeshMaterials(colorData.Colors[selectedColor]);
        }

        private void Hold()
        {
            outlineObject.layer = LayerMask.NameToLayer("Outline");
            _stickmanAnimationController.PlayAnim(StickmanAnimTypes.WAVE);
        }
        private void Release()
        {
            outlineObject.layer = LayerMask.NameToLayer("NoOutline");
            _stickmanAnimationController.PlayAnim(StickmanAnimTypes.IDLE, 2);
        }

        public Point OnClick() => _stickmanPoint;
        public List<Point> PointsList() => new List<Point>(1) { _stickmanPoint };

        public void SetStickmanPoint(Point stickmanPoint) => _stickmanPoint = stickmanPoint;
        public bool SetTargetPoint(Point targetPoint)
        {
            if (!GridController.Instance.IsWaypointAvailable(targetPoint)) return false;
            _targetPoint = targetPoint;
            return FindPath();
        }

        #endregion

        #region PATHFINDING

        public void FindBestCarPosition(List<Point> points, CarController carController)
        {
            List<Point> bestPath = new List<Point>();
            int bestPointIndex = 0;


            //HATA VERER BURASI AKTÝF OLARAK
            if (points[0] != null && points[0] == _stickmanPoint)
            {
                bestPath.Add(points[0]);
            }
            else if(points.Count > 1 && points[1] != null && points[1] == _stickmanPoint)
            {
                bestPath.Add(points[1]);
            }
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (!GridController.Instance.IsWaypointAvailable(points[i]))
                    {
                        continue;
                    }
                    if (bestPath.Count == 0)
                    {
                        bestPointIndex = i;
                        bestPath = Pathfinding.FindPath(GridController.Instance.GetMatrix(), _stickmanPoint, points[i]);
                    }
                    else
                    {
                        List<Point> newPath = Pathfinding.FindPath(GridController.Instance.GetMatrix(), _stickmanPoint, points[i]);
                        if (newPath.Count > 0 && newPath.Count < bestPath.Count)
                        {
                            bestPointIndex = i;
                            bestPath = newPath;
                        }
                    }
                }
            }
            

            targetPath = bestPath;

            if (targetPath.Count > 0)
            {
                _carController = carController;

                GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, true);
                GridController.Instance.SetGroundColor(points[bestPointIndex], SelectedColor.Green);
                IsHold = false;

                _stickmanAnimationController.PlayAnim(StickmanAnimTypes.RUN);
                SetEmotion(SelectedEmotion.HAPPY);
            }
            else
            {
                GridController.Instance.SetGroundColor(points[Random.Range(0,2)], SelectedColor.Red);
            }
        }

        public bool FindPath()
        {
            List<Point> newPath = Pathfinding.FindPath(GridController.Instance.GetMatrix(), _stickmanPoint, _targetPoint);
            if (newPath.Count <= 0) return false;

            targetPath.Clear();
            GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, true);
            targetPath = newPath;
            IsHold = false;
            _onMove = true;
            _stickmanAnimationController.PlayAnim(StickmanAnimTypes.RUN);
            return true;
        }

        private int currentTargetIndex = 0;
        public float moveSpeed = 1;
        private void Update()
        {
            if (targetPath.Count == 0)
            {
                return;
            }

            Point currentPoint = targetPath[currentTargetIndex];
            _stickmanPoint = currentPoint;
            GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, false);
            Vector3 worldPos = GridController.Instance.GridToWorlPosition(currentPoint);
            Vector3 targetPosition = new Vector3(worldPos.x, transform.position.y, worldPos.z);

            transform.LookAt(targetPosition);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, true);
                SetNextTarget();
            }
        }

        private void SetNextTarget()
        {
            currentTargetIndex++;
            if (currentTargetIndex >= targetPath.Count)
            {
                targetPath.Clear();
                currentTargetIndex = 0;
                _onMove = false;
                IsHold = false;
                if (_carController)
                {
                    GetInCar(_carController.carTransform);
                    _carController.carAnimationController.FindNearestDoor(transform.position);
                    
                    GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, true);
                }
                else
                {
                    GridController.Instance.UpdateMatrix(_stickmanPoint.x, _stickmanPoint.y, false);
                }
                
            }
        }

        #endregion

        #region CAR INTERACTIONS

        private void GetInCar(Transform carTransfrom)
        {
            transform.DOLookAt(carTransfrom.position, 0.2f);
            _stickmanAnimationController.PlayAnim(StickmanAnimTypes.ENTER_CAR);
            transform.DOLocalMove(carTransfrom.position, 1f).OnComplete((() => Destroy(gameObject)));
            transform.DOScale(new Vector3(0.33f, 0.3f, 0.3f), 1.0f);
        }

        #endregion

        #region EMOTION CONTROL

        public void SetEmotion(SelectedEmotion selectedEmotion) => emotionController.ShowEmotion(selectedEmotion);


        #endregion
    }
}
