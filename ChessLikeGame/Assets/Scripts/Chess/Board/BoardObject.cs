using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Chess.Board
{
    public class BoardObject : MonoBehaviour
    {
        public readonly List<List<Position>> Cubes = new List<List<Position>>();
        public int rows = 10;
        public int columns = 10;
        [SerializeField] private PositionGameObject cube;
        private Vector3 _cubeSize;
        public event Action OnBoardSetUp;

        public List<List<Position>> GetHypotheticalBoard()
        {
            List<List<Position>> posList = new List<List<Position>>();
            foreach (List<Position> pList in Cubes)
            {
                List<Position> newList = new List<Position>();
                foreach (Position p in pList)
                {
                    newList.Add(new Position(p));
                }
                posList.Add(newList);
            }
            return posList;
        }

        public void ClearBoard()
        {
            foreach (List<Position> posList in Cubes)
            {
                foreach (Position pos in posList)
                {
                    pos.Deactivate();
                }
            }
        }
        private void Start()
        {
            GameObject cubeObj = cube.gameObject;
            _cubeSize = cubeObj.transform.localScale;
            for (int c = 0; c < columns; c++)
            {
                List<Position> objects = new List<Position>();
                for (int r = 0; r < rows; r++)
                {
                    GameObject obj = Instantiate(cubeObj, transform);
                    obj.transform.position = (transform.position - new Vector3(0,(_cubeSize.y / 2),0)) -
                        new Vector3((_cubeSize.x * c), 0, (_cubeSize.z * r));
                    objects.Add(obj.GetComponent<PositionGameObject>().GetPosition());
                    objects[r].grid = new Vector2(c, r);
                }
                Cubes.Add(objects);
            }

            OnBoardSetUp?.Invoke();
        }
        
        
    }
}