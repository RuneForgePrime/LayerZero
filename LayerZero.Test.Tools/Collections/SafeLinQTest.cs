using FluentAssertions;
using LayerZero.Tools.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Test.Tools.Collections
{
    public class SafeLinQTest
    {
        #region elementAtSafe
        [Fact]
        public void ElementAtSafe_NullSource()
        {
            List<int> ints = null;
            int Index = 1;
            FluentActions
                .Invoking(() => SafeLinQ<int>.ElementAtSafe(ints, Index))
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"Value cannot be null. (Parameter '{nameof(ints)}')");
        }

        [Fact]
        public void ElementAtSafe_Empty() {
            List<int> ints = new List<int>();
            int Index = 0;

            FluentActions
                .Invoking(() => SafeLinQ<int>.ElementAtSafe(ints, Index))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage($"Parameter {nameof(ints)} cannot be empty");
        }

        [Fact]
        public void ElementAtSafe_negativeIndex()
        {
            var ints = new List<int> { 1, 2, 3 };
            int Index = -1;

            FluentActions
                .Invoking(() => SafeLinQ<int>.ElementAtSafe(ints, Index))
                .Should()
                .Throw<IndexOutOfRangeException>()
                .WithMessage($"{nameof(Index)} refers to an index out of bounds (Count: {ints.Count}).");
        }

        [Fact]
        public void ElementAtSafe_outOfBoundindex()
        {
            var ints = new List<int> { 1, 2, 3 };
            int Index = ints.Count() + 10;

            FluentActions
                .Invoking(() => SafeLinQ<int>.ElementAtSafe(ints, Index))
                .Should()
                .Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void ElementAtSafe_IList_returnSingle() {
            var ints = new List<int> { 1, 2, 3 };
            int Index = 0;

            SafeLinQ<int>.ElementAtSafe(ints, Index)
                .Should()
                .Be(ints.First());
        }

        [Fact]
        public void ElementAtSafe_Not_IList_returnSingle()
        {
            var ints = new HashSet<int> { 1, 2, 3 };
            int Index = 1;

            SafeLinQ<int>.ElementAtSafe(ints, Index)
                .Should()
                .Be(2);
        }
        #endregion
    }
}
