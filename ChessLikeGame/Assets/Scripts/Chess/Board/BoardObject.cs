using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Board
{
    public class BoardObject : MonoBehaviour
    {
        public List<List<Position>> _cubes = new List<List<Position>>();
        public int rows = 10;
        public int columns = 10;
        [SerializeField] Position cube;
        private Vector3 cubeSize;
        public event Action OnBoardSetUp;

        private void Start()
        {
            GameObject cubeObj = cube.gameObject;
            cubeSize = cubeObj.transform.localScale;
            for (int c = 0; c < columns; c++)
            {
                List<Position> objects = new List<Position>();
                for (int r = 0; r < rows; r++)
                {
                    GameObject obj = Instantiate(cubeObj, transform);
                    obj.transform.position = (transform.position + new Vector3(0,(cubeSize.y / 2),0)) -
                        new Vector3((cubeSize.x * c), 0, (cubeSize.z * r));
                    objects.Add(obj.GetComponent<Position>());
                    objects[r].grid = new Vector2(c, r);
                }
                _cubes.Add(objects);
            }

            OnBoardSetUp?.Invoke();
        }
    }
}