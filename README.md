# PSM

PlayStation Mobile 概要
PlayStation Mobile はソニーコンピュータエンタテイメント（SCE）が提唱する、新しいコンテンツ配信の取り組みです。発表当時は PlayStation Mobile と命名していましたが、その後 PlayStation Mobile に改名されました。PlayStation Mobile 対応コンテンツは PlayStation Store を通じて PlayStation Vita や SCE がライセンスする Android 端末に配信できます。

図1 PlayStation Mobile
図1 PlayStation Mobile
開発者は PlayStation Mobile Developer Program で有料の契約（年間 US$99 相当を予定）を結び、開発したアプリケーションを配信できます。ただし、本稿執筆時の2013年5月8日から、期間限定で無料で申し込み可能となっています。詳細は PlayStation Mobile 公式サイトをご覧ください。

これまで PLAYSTATION3 や PlayStation Vita のようなゲーム専用端末のコンテンツを開発するには、SCE と法人契約した企業にのみ限られていましたが、PlayStation Mobile は iOS や Android などのスマートフォンのアプリケーション配信ストアに近いビジネスモデルになると予想され、個人の開発者も PlayStation Vita などの PlayStation Mobile 対応デバイスで実行できるプログラムを開発し、PlayStation Store でコンテンツを販売できるようになるでしょう。

PlayStation Certified デバイス
PlayStation Mobile アプリケーションは PlayStation Vitaで実行できることが最大の特徴ですが、SCE がライセンスする PlayStation Certified デバイスでも実行できます。

本校執筆時点で発表されている対応デバイスは Xperia や Sony Tablet シリーズなど Sony 製の Android デバイスが中心ですが、加えて HTC の HTC Oneシリーズが対応しています。詳細は公式サイトの対応機種一覧を見てください。

PlayStation™Certified devices

PlayStation Mobileの技術
PlayStation Mobile は上記のように PlayStation Vita や PlayStation Certified デバイスなどクロスプラットフォームで動作するアプリケーション実行環境です。

PlayStation Mobile SDK で開発されたアプリケーションは、コードを変更することなく多様なデバイスで実行できます。 このクロスプラットフォーム性は PlayStation Mobile が Mono と呼ばれるオープンソースで開発されている仮想マシンによって実現しています。

図2 PlayStation Mobileアーキテクチャ概要
図2 PlayStation Mobileアーキテクチャ概要
通常、プログラミング言語で書かれたコードはコンパイラによって対象のデバイスおよび OS に特化した機械語に変換されます。機械語に変換された実行可能ファイルは、対象の CPU 及び OS 以外で動かすことはできません。

1990 年代中ごろから、Java の普及と共に実行環境に依存しない中間言語方式が注目されるようになりました。中間言語方式では、プログラミング言語で書かれたコードが特定の CPU や OS に依存しない中間言語と呼ばれる仮想的な機械語に変換されます。そして、仮想マシンと呼ばれる中間言語を読み取り、機械語に変換して実行するソフトウェアによって実行されます。

PlayStation Mobile は Mono の仮想マシンをベースに、ゲームやアプリケーション開発に特化したフレームワークを提供することで、PlayStation Mobile に対応している環境であれば同じように実行できるように仕組んでいるのです。 

基盤技術はMicrosoft .NET Framework
PlayStation Mobile の技術基盤である Mono は Microsoft が開発する .NET Framework と呼ばれる技術と互換性のあるオープンソースの実装です。.NET Framework は多くの Windows アプリケーションが基盤としている技術で、それ自体は Microsoft の製品ですが、仕様はオープンに公開されています。

PlayStation Mobile SDK が C# を開発言語としているのも .NET Framework アプリケーションの主要な開発言語が C# であるためです。

Microsoft は家庭用ゲーム機 Xbox 360 で SCE と競合しているため、SCE が Microsoft の技術である C# と .NET Framework を採用したことに驚きの声も少なくありません。しかし、これは SCE が技術的に Microsoft に飲み込まれたわけではありません。

C# 及び .NET Framework は国際標準技術として仕様が公開されているオープンな技術であり、その内容は各種標準化団体で認定されています。例えば C# は ISO/IEC 23270 で、中間言語とそれを実行する共通言語基盤（仮想マシン）は ISO/IEC 23271 で定められています。

PlayStation Mobile や Unity の実行基盤となっている Mono は、こうした公開されている標準技術に基づいてオープンソースで開発された .NET Framework 互換環境なのです。

特に 1990 年代の Microsoft を知る技術者であれば独善的な企業というイメージが強いため、プロプライエタリな独自技術にロックインされるのではないかと警戒でしょう。しかし 2000 年代中ごろから Microsoft の方針が変化し、多くの自社技術をオープンソース化するほか、標準化やオープンソースの支援を積極的に行うようになりました。PlayStation Mobile や Unity など、非 Windows 環境で互換技術の採用が進んでいることが、その証左となっています。

Unity との共通点と相違点
PlayStation Mobile アプリケーションの開発環境と実行基盤の仕組みは Unity に非常によく似ています。実行基盤に Mono を採用することで OS やデバイスを抽象化する点だけではなく、標準の統合開発環境 PlayStation Mobile Studio が MonoDevelop をベースとする点も同じです。

図3 Unityに付属するMonoDevelop
図3 Unityに付属するMonoDevelop
図4 PlayStation Mobile Studio
図4 PlayStation Mobile Studio
MonoDevelop は、それ自身が Mono 上で動作する統合開発環境で、Mono 及び .NET Framework 上で動作するアプリケーション開発の為に作られました。

PlayStation Mobile Studio は MonoDevelop をカスタマイズした PlayStation Mobile アプリケーション開発に特化した統合開発環境です。C# エディタなどの基本的な部分は純正の MonoDevelop と大きな違いはありませんが、専用のソリューション・テンプレート「PlayStation Mobile アプリケーション」が用意されています。

図5 PlayStation Mobile アプリケーションのソリューション
図5 PlayStation Mobile アプリケーションのソリューション
「PlayStation Mobile アプリケーション」から作成したソリューションは、開発したアプリケーションをPCや接続されているデバイスで実行できます。プログラムを PlayStation Vita や PlayStation Certified 端末で動かすには、端末側に PlayStation Mobile Development Assistant をインストールしておく必要があります。

PS Vita 用の PlayStation Mobile Development Assistant は PlayStation Store で配信されています。PS Vita から開発者ライセンスを登録しているアカウントで PlayStation Store にアクセスしてください。

Android 用の PlayStation Mobile Development Assistant は SDK に含まれているため、PlayStation Mobile Studio で実行すると自動的に端末側にインストールされます。

図6 実行環境の選択
図6 実行環境の選択
PC にデバイスを接続し、正しく認識されていれば PlayStation Mobile Studio のツールバーにあるコンボボックスでデバイスを選択できます。PlayStation Vita や PlayStation Certified デバイスが無くても、「PlayStation Mobile Simulator」を選択することで、PC上でアプリケーションを実行できます。

MonoDevelop をベースとした統合開発環境を使い、C# で開発する点は Unity と同じですが、開発方法は異なります。Unity はシーンエディタを用いてオブジェクトを空間に配置し、グラフィカルにゲーム世界を構築できました。一方、PlayStation Mobile Studio はコードによる開発が中心となります。ゲームループを管理し、ゲームの更新や描画処理を自前で記述する必要があります。

Unity と比較した場合、主に 3D ゲーム開発に特化している Unity に対し、PlayStation Mobile SDK はゲームに限らず一般的なツール系アプリケーションの開発も対象としています。Unity は得意分野が限定されており、3D 空間と物理エンジンを応用したゲームは作りやすいのですが、構造的なデータ管理や UI 設計が不得手な側面があります。

PlayStation Mobile SDK は Unity のシーンエディタのようなゲーム空間を構築するツールはありませんが、代わりに UI Toolkit と呼ばれるユーザーインターフェイスに特化したライブラリが提供されます。加えてUIデザインをサポートするツール UI Composer が付属しており、効率的に UI のデザインが行えます。

基本的なアプリケーション構造
PlayStation Mobile SDK のアーキテクチャは実行基盤を Mono としている点において Unity に似ていますが、開発方法は DirectX や XNA Framework などで開発するゲームのように、すべてをコードで記述しなければなりません。グラフィックスに関連するフレームワークは DirectX や OpenGL の開発経験があれば違和感のない設計になっているので、一定のゲーム開発経験があれば習得は難しくありません。

ここでは、単純に背景色を黒から赤に変化するアニメーション処理を例に基本的な動きを確認します。

コード1
public class AppMain
{
	private static bool isRunning;
	private static bool isInc;
	private static int red;
	private static GraphicsContext graphics;

	public static void Main (string[] args)
	{
		Initialize ();
		while (isRunning) 
{
			SystemEvents.CheckEvents ();
			Update ();
			Render ();
		}
	}

	public static void Initialize ()
	{
		graphics = new GraphicsContext ();
		isRunning = true;
		isInc = true;
	}

	public static void Update ()
	{
		GamePadData gamePadData = GamePad.GetData (0);
		if ((gamePadData.ButtonsDown & GamePadButtons.Select) == GamePadButtons.Select)
		{
			isRunning = false;
		}
		
		red = isInc ? red + 1 : red - 1;
		if (red == 255) isInc = false;
		else if (red == 0) isInc = true;
	}

	public static void Render ()
	{
		graphics.SetClearColor(red, 0, 0, 255);
		graphics.Clear ();

		graphics.SwapBuffers ();
	}
}
プログラムは Main() メソッドから開始し、最初に Initialize() メソッドを呼び出します。Initialize() メソッドは起動時に一度だけ呼び出されるので、ここに初期化処理を記述します。

Initialize() メソッドの処理が終了し Main() メソッドに復帰すると while 文でゲームの更新と描画処理の繰り返しを行います。いわゆるゲームループです。ここでは SystemEvents.CheckEvents() メソッドで OS に依存するイベントの検出を行います。例えば Windows であれば、このメソッドの内部でウィンドウメッセージの処理を行っています。

続いて Update() メソッドと Render() メソッドが順に呼び出されます。通常 Update() メソッドでゲームデータの更新を行い、Render() メソッドで描画を行います。Update() メソッドで CPU を用いた計算を行い、Render() メソッドで GPU による描画を行うという役割分担です。

ゲームループはゲームが終了するまで永遠と繰り返されるもので、ゲームに関連するコードの大部分は、ここから呼び出される Update() メソッドと Render() メソッドの流れの中に記述されます。

上のコードでは isRunning フィールドが true であればゲームループを繰り返すようにプログラムしています。Update() メソッド内でゲームパッドの状態を調べ、Select ボタンが押されていれば isRunning フィールドを false に変更してゲームを終了させます。

グラフィックスに関連した処理は Initialize() メソッドでインスタンス化している GraphicsContext オブジェクトを用います。Render() メソッドでは GraphicsContext オブジェクトの SetClearColor() メソッドで画面（フレームバッファ）をクリアする色を設定し、Clear() メソッドでクリアしています。このとき SetClearColor() メソッドに設定する色の赤要素に red フィールドを設定しているため、Update() メソッドで red フィールドの値を変更することで、画面を塗りつぶす色が変化するという仕掛けです。

最後の SwapBuffers() メソッドで描画した結果を反映させます。GraphicsContext クラスのレンダリングはダブルバッファ方式で、SwapBuffers() メソッドが呼び出されるまでの描画過程が画面に表示されることはありません。SwapBuffers() メソッドが実行されたタイミングで、現在の画面に表示されているフレームバッファと交換されます。

このときディスプレイの更新とフレームバッファの更新がずれるとティアリングと呼ばれる画面のちらつきが発生してしまいます。これを防ぐために GraphicsContext はディスプレイの更新間隔に合わせて SwapBuffers() メソッドを待機させます。この設定を垂直同期と呼び、一般的なデバイスでは 1 秒間に 60 回（約 16.6 ミリ秒に 1 回）の間隔で画面を更新します。Update() メソッドと Render() メソッドが呼び出される間隔はディスプレイの垂直同期に合わせられるため、それぞれ 1 秒間に約 60 回のペースで呼び出されることになります。

UI Composer と UI Toolkit
PlayStation Mobile SDK ではアプリケーションの UI に利用できるウィジェット（Widget）と呼ばれる部品群を UI Toolkit というライブラリで提供しています。ラベル、ボタン、テキスト編集、イメージ、スライダーなどのウィジェットが標準で用意されており、必要に応じてカスタムウィジェットを作成することも可能です。

図7 標準のウィジェット
図7 標準のウィジェット
UI Toolkit によってウィジェットのインスタンスを作成するだけでボタンなどの UI 要素を表示できますが、ウィジェットの位置やデザインを変更するたびにコードを書き換え、ビルドして実行するのは極めて効率が悪いです。

そこで UI のデザインを専門とする UI Composer というツールが提供されています。

図8 UI Composer
図8 UI Composer
UI Composer を使うとグラフィカルなデザイナ上でウィジェットを編集でき、マウスのドラッグ & ドロップを中心とする操作だけでアプリケーションの UI をデザインができます。

また UI Composer は多言語対応のために言語テーブルを作成することができます。ウィジェットから言語テーブルを参照することで、UIで使用するテキストを言語ごとに切り替えられます。

図9 言語テーブルによる多言語対応
図9 言語テーブルによる多言語対応
UI Composer で配置したウィジェットは C# コードとして出力され、PlayStation Mobile Studio のプロジェクトに追加して利用できます。

UI Composer による UI のデザインは便利ですが PlayStation Mobile Studio に統合されておらず、プロジェクトも別に管理されるため PlayStation Mobile Studio のプロジェクトとは同期されません。実質的には UI コードのジェネレータ程度のもので、Visual Studio のような使い勝手を想像すると期待を外れます。ウィジェットに対するイベントハンドラは自己管理しなければなりません。

当然、UI Composer でデザインできる範囲はビルド時のインスタンス構造のみで、実行時に UI が変化する場合はコードで記述しなければなりません。

PlayStation Mobile と Android
PlayStation Mobile アプリケーションは PlayStation Vita で実行できるという点が売りになっていますが、デバイスの普及台数を考えれば PlayStation Certified ライセンスを取得した Android デバイスで実行できる点も無視できません。

Android は開発者にとって自由度が高い OS ですが、その一方で性能の異なるハードウェアが大量に作られ、バージョンや機能もデバイスごとに異なる状態が続いています。一定の性能が要求されるゲーム開発にとって、これは致命的な問題です。これに対し PlayStation Certified は SCE が定める基準を満たしたデバイスにのみライセンスされるため、PlayStation Mobile アプリケーションを安定して実行できる性能が保証されます。

しかし PlayStation Certified ライセンスを取得するデバイスは Android デバイス全体で見ればごく一部なので、動作環境は限られてしまいます。PlayStation Vita の普及台数も十分とは言えないため、PlayStation Mobile が iOS や Android のゲーム開発者にとって魅力的な市場になるかどうかは未知数です。

表1 PlayStation Mobile と Android の比較
 	PlayStation Mobile	Android
アプリ配信	Play Station Store	 
審査	ガイドラインに基づいた審査	プログラムによる自動審査
開発者ライセンス	USD 99 / 年	USD 25
主な開発環境	PlayStation Mobile Studio	Eclipse, Unity
主な開発言語	C#	Java（Eclipse）
C#, JavaScript（Unity）
実行環境	PlayStation Vita
PlayStation Certified デバイス	Android OS
普及台数や市場規模で見れば、PlayStation Mobile は始まったばかりであり、既存のスマートフォンよりもまだ小さく見えますが、個人開発者や小規模な開発組織がゲーム専用端末である PlayStation Vitaで動作するアプリケーションを開発できるようになるというのは画期的なことです。アプリケーションが配信される PlayStation Store は、すでに有料のゲームや追加コンテンツを配信するマーケットとして実績があり、ユーザーが有料のコンテンツを購入することに慣れているという点も重要です。

スマートフォンのアプリケーションはすでに飽和状態であり、新しいアプリケーションを配信しても、何らかの形で露出しなければユーザーがアプリケーションを見つけてくれません。PlayStation Mobile はスマートフォンやソーシャル系のアプリケーション開発者にとって、小規模に参入できる新しいチャンスとなるでしょう。

