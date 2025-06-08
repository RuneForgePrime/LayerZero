using FluentAssertions;
using LayerZero.Tools.Managements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Test.Tools.Management
{
    public class GeasMasterTests
    {
        [Fact]
        public void RunSync_WithTaskReturningValue_ShouldReturnExpectedResult()
        {
            // Arrange
            Func<Task<int>> asyncTask = async () =>
            {
                await Task.Delay(10);
                return 42;
            };

            // Act
            int result = GeasMaster.RunSync(asyncTask);

            // Assert
            result.Should().Be(42);
        }

        [Fact]
        public void RunSync_WithVoidTask_ShouldExecuteSuccessfully()
        {
            // Arrange
            bool wasCalled = false;
            Func<Task> asyncTask = async () =>
            {
                await Task.Delay(10);
                wasCalled = true;
            };

            // Act
            GeasMaster.RunSync(asyncTask);

            // Assert
            wasCalled.Should().BeTrue();
        }

        [Fact]
        public void RunSync_WhenTaskThrows_ShouldPropagateException()
        {
            // Arrange
            Func<Task> asyncTask = async () =>
            {
                await Task.Delay(5);
                throw new InvalidOperationException("Expected failure");
            };

            // Act
            Action act = () => GeasMaster.RunSync(asyncTask);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Expected failure");
        }

        [Fact]
        public void RunSyncT_WhenTaskThrows_ShouldPropagateException()
        {
            // Arrange
            Func<Task<int>> asyncTask = async () =>
            {
                await Task.Delay(5);
                throw new ArgumentException("Invalid argument");
            };

            // Act
            Action act = () => GeasMaster.RunSync(asyncTask);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Invalid argument");
        }

        [Fact]
        public void RunSync_WithCancellationToken_TaskCancelled_ShouldThrow()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Func<CancellationToken, Task> asyncTask = async ct =>
            {
                await Task.Delay(5, ct);
            };

            // Act
            Action act = () => GeasMaster.RunSync(asyncTask, cts.Token);

            // Assert
            act.Should().Throw<TaskCanceledException>();
        }

        [Fact]
        public void RunSyncT_WithCancellationToken_TaskCancelled_ShouldThrow()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Func<CancellationToken, Task<int>> asyncTask = async ct =>
            {
                await Task.Delay(5, ct);
                return 1;
            };

            // Act
            Action act = () => GeasMaster.RunSync(asyncTask, cts.Token);

            // Assert
            act.Should().Throw<TaskCanceledException>();
        }

        [Fact]
        public void RunSyncT_WithCancellationToken_ShouldReturnExpectedResult()
        {
            // Arrange
            using var cts = new CancellationTokenSource();

            Func<CancellationToken, Task<int>> asyncTask = async ct =>
            {
                await Task.Delay(5, ct);
                return 99;
            };

            // Act
            int result = GeasMaster.RunSync(asyncTask, cts.Token);

            // Assert
            result.Should().Be(99);
        }

        [Fact]
        public void RunSync_WithCancellationToken_ShouldExecuteSuccessfully()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            bool wasExecuted = false;

            Func<CancellationToken, Task> asyncTask = async ct =>
            {
                await Task.Delay(5, ct);
                wasExecuted = true;
            };

            // Act
            GeasMaster.RunSync(asyncTask, cts.Token);

            // Assert
            wasExecuted.Should().BeTrue();
        }
    }
}
