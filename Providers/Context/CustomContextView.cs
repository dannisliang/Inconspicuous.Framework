#pragma warning disable 0649

using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Inconspicuous.Framework {
	public sealed class CustomContextView : ContextView {
		[SerializeField]
		[Type(typeof(IContext))]
		private string contextType;

		[SerializeField]
		[Type(typeof(IContext))]
		private string[] subContextTypes;

		public override void Initialize() {
			if(!CheckAndRemoveDuplicate()) {
				if(subContextTypes.Length == 0) {
					Context = Initialize(Type.GetType(contextType));
					Context.Start();
				} else {
					var loadingSubContexts = subContextTypes
						.Select(t => LoadSceneForContext(Type.GetType(t))).ToArray();
					Observable.WhenAll(loadingSubContexts)
						.Select(subContexts => subContexts.Cast<Context>().ToArray())
						.ObserveOnMainThread()
						.Subscribe(subContexts => {
							Context = Initialize(Type.GetType(contextType), subContexts);
							Context.Start();
						}).DisposeWith(this);
				}
			}
		}
	}
}
