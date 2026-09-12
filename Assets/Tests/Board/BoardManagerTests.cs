using System.Collections;
using Board;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BoardTests
{
    public class BoardManagerTests
    {
        private GameObject m_GameObject;
        private BoardManager m_Board;

        [SetUp]
        public void SetUp()
        {
            m_GameObject = new GameObject("BoardTest");

            m_Board = m_GameObject.AddComponent<BoardManager>();
            m_Board.Init(8, 8);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(m_GameObject);

        private class FakeOccupant : ICellOccupant
        {
            public GameObject GameObject => null;
        }

        private static Vector2Int Cell(int x, int y) => new Vector2Int(x, y);

        [Test]
        public void Init_SetsWidthAndHeight()
        {
            Assert.AreEqual(8, m_Board.Width);
            Assert.AreEqual(8, m_Board.Height);
        }

        [TestCase(0, 0, true, TestName = "Bottom-left corner is in bounds")]
        [TestCase(7, 7, true, TestName = "Top-right corner is in bounds")]
        [TestCase(3, 4, true, TestName = "Interior cell is in bounds")]
        [TestCase(-1, 0, false, TestName = "Negative x is out of bounds")]
        [TestCase(8, 0, false, TestName = "x at width is out of bounds")]
        [TestCase(0, -1, false, TestName = "Negative y is out of bounds")]
        [TestCase(0, 8, false, TestName = "y at height is out of bounds")]
        [TestCase(-5, -5, false, TestName = "Far out is out of bounds")]
        public void GetCellData_ReturnsNonNullOnlyForInBoundsCells(int x, int y, bool inBounds)
        {
            BoardManager.CellData data = m_Board.GetCellData(Cell(x, y));

            Assert.AreEqual(inBounds, data != null);
        }

        [Test]
        public void SetCellPassable_UpdatesPassableFlag()
        {
            Vector2Int cell = new Vector2Int(3, 3);

            m_Board.SetCellPassable(cell, true);

            Assert.IsTrue(m_Board.GetCellData(cell).Passable);
        }

        [Test]
        public void SetOccupant_ThenGetOccupantAt_ReturnsSameOccupant()
        {
            Vector2Int cell = new Vector2Int(2, 3);
            FakeOccupant occupant = new FakeOccupant();

            m_Board.SetOccupant(cell, occupant);

            Assert.AreSame(occupant, m_Board.GetOccupantAt(cell));
        }

        [Test]
        public void RemoveOccupant_WithMatchingOccupant_ClearsCell()
        {
            Vector2Int cell = new Vector2Int(1, 1);
            FakeOccupant occupant = new FakeOccupant();

            m_Board.SetOccupant(cell, occupant);
            m_Board.RemoveOccupant(cell, occupant);

            Assert.IsNull(m_Board.GetOccupantAt(cell));
        }

        [Test]
        public void RemoveOccupant_WithDifferentOccupant_LeavesCellUntouched()
        {
            Vector2Int cell = new Vector2Int(1, 1);
            FakeOccupant existing = new FakeOccupant();
            FakeOccupant intruder = new FakeOccupant();

            m_Board.SetOccupant(cell, existing);
            m_Board.RemoveOccupant(cell, intruder);

            Assert.AreSame(existing, m_Board.GetOccupantAt(cell));
        }

        [Test]
        public void RemoveOccupant_OnEmptyCell_DoesNotThrow() =>
            Assert.DoesNotThrow(() =>
                m_Board.RemoveOccupant(new Vector2Int(0, 0), new FakeOccupant())
            );

        [Test]
        public void ClearCell_RemovesOccupant()
        {
            Vector2Int cell = new Vector2Int(4, 4);
            m_Board.SetOccupant(cell, new FakeOccupant());

            m_Board.ClearCell(cell);

            Assert.IsNull(m_Board.GetOccupantAt(cell));
        }

        [Test]
        public void ClearCell_OutOfBounds_DoesNotThrow() =>
            Assert.DoesNotThrow(() => m_Board.ClearCell(new Vector2Int(-1, -1)));

        [Test]
        public void IsCellOccupiable_PassableAndEmpty_ReturnsTrue()
        {
            Vector2Int cell = new Vector2Int(2, 2);
            m_Board.SetCellPassable(cell, true);

            Assert.IsTrue(m_Board.IsCellOccupiable(cell));
        }

        [Test]
        public void IsCellOccupiable_NotPassable_ReturnsFalse()
        {
            Vector2Int cell = new Vector2Int(2, 2);
            m_Board.SetCellPassable(cell, false);

            Assert.IsFalse(m_Board.IsCellOccupiable(cell));
        }

        [Test]
        public void IsCellOccupiable_HasOccupant_ReturnsFalse()
        {
            Vector2Int cell = new Vector2Int(2, 2);
            m_Board.SetCellPassable(cell, true);
            m_Board.SetOccupant(cell, new FakeOccupant());

            Assert.IsFalse(m_Board.IsCellOccupiable(cell));
        }

        [Test]
        public void IsCellOccupiable_OutOfBounds_ReturnsFalse() =>
            Assert.IsFalse(m_Board.IsCellOccupiable(new Vector2Int(-1, -1)));

        [Test]
        public void IsInLineOfSight_AdjacentHorizontal_ReturnsTrue() =>
            Assert.IsTrue(m_Board.IsInLineOfSight(new Vector2Int(3, 3), new Vector2Int(4, 3)));

        [Test]
        public void IsInLineOfSight_AdjacentVertical_ReturnsTrue() =>
            Assert.IsTrue(m_Board.IsInLineOfSight(new Vector2Int(3, 3), new Vector2Int(3, 4)));

        [Test]
        public void IsInLineOfSight_TwoCellsApartWithClearPath_ReturnsTrue() =>
            Assert.IsTrue(m_Board.IsInLineOfSight(new Vector2Int(1, 3), new Vector2Int(4, 3)));

        [Test]
        public void IsInLineOfSight_Diagonal_ReturnsFalse() =>
            Assert.IsFalse(m_Board.IsInLineOfSight(new Vector2Int(3, 3), new Vector2Int(4, 4)));

        [Test]
        public void IsInLineOfSight_ObstructedByOccupant_ReturnsFalse()
        {
            m_Board.SetOccupant(new Vector2Int(2, 3), new FakeOccupant());

            Assert.IsFalse(m_Board.IsInLineOfSight(new Vector2Int(1, 3), new Vector2Int(4, 3)));
        }
    }
}
