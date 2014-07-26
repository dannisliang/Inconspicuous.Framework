using System;
using System.ComponentModel.Composition;
using DryIoc.MefAttributedModel;
using UniRx;
using UnityEngine;

namespace Inconspicuous.Framework.Example.Test {
	[ExportAll]
	public class TestFactory : IFactory<Func<int, Test>> {
		[Export]
		public Func<int, Test> Create() {
			return i => new Test { Name = i.ToString() };
		}
	}

	[Export(typeof(IMediator<TestView>))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class TestMediator : Mediator<TestView> {
		private readonly ICommandDispatcher commandDispatcher;
		private readonly TestViewModel testViewModel;

		public TestMediator(
			ICommandDispatcher commandDispatcher,
			Func<GameObjectData, GameObject> gameObjectFactory,
			TestViewModel testViewModel,
			Func<int, Test> testFactory) {
			this.commandDispatcher = commandDispatcher;
			this.testViewModel = testViewModel;
			var test = testFactory(300);
			Debug.Log(test.Name);
		}

		public override void Mediate(TestView view) {
			testViewModel.ButtonSignal.Subscribe(_ => {
				commandDispatcher.Dispatch(new TestCommand {
					FirstValue = 3,
					SecondValue = "Testadf"
				}).Subscribe().DisposeWith(this);
			}).DisposeWith(this);
			view.ButtonSignal
				.Subscribe(_ => {
					//var item = itemRepository.Items.Count;
					testViewModel.ButtonSignal.Dispatch();
				}).DisposeWith(this);
		}
	}
}
