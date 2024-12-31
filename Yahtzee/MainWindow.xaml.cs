using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yahtzee;

public partial class MainWindow : Window
{
	private Grid leftYahtzeeGrid;
	private readonly Dictionary<Button, int> leftDice = new();
	private readonly Dictionary<Button, int> rightDice = new();
	private readonly Random random = new();
	private int rollCount = 0;

	public MainWindow()
	{
		InitializeComponent();

		Grid mainGrid = new Grid
		{
			Margin = new Thickness(10),
			Background = new SolidColorBrush(Colors.White)
		};

		mainGrid.RowDefinitions.Add(new RowDefinition
			{ Height = new GridLength(1, GridUnitType.Auto) }); // Рядок управління грою
		mainGrid.RowDefinitions.Add(new RowDefinition
			{ Height = new GridLength(5, GridUnitType.Star) }); // Основна частина
		mainGrid.RowDefinitions.Add(new RowDefinition
			{ Height = new GridLength(1, GridUnitType.Star) }); // Рядок для квадратиків
		mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

		Grid controlGrid = CreateControlGrid();
		leftYahtzeeGrid = CreateYahtzeeGrid(true);
		Grid rightYahtzeeGrid = CreateYahtzeeGrid(false);

		Grid.SetColumn(controlGrid, 0);
		Grid.SetColumnSpan(controlGrid, 2);
		Grid.SetRow(controlGrid, 0);
		Grid.SetColumn(leftYahtzeeGrid, 0);
		Grid.SetRow(leftYahtzeeGrid, 1);
		Grid.SetColumn(rightYahtzeeGrid, 1);
		Grid.SetRow(rightYahtzeeGrid, 1);

		Grid leftSquaresGrid = CreateSquaresGrid(true);
		Grid rightSquaresGrid = CreateSquaresGrid(false);

		Grid.SetColumn(leftSquaresGrid, 0);
		Grid.SetRow(leftSquaresGrid, 2);
		Grid.SetColumn(rightSquaresGrid, 1);
		Grid.SetRow(rightSquaresGrid, 2);

		mainGrid.Children.Add(controlGrid);
		mainGrid.Children.Add(leftYahtzeeGrid);
		mainGrid.Children.Add(rightYahtzeeGrid);
		mainGrid.Children.Add(leftSquaresGrid);
		mainGrid.Children.Add(rightSquaresGrid);

		this.Content = mainGrid;
	}

	private Grid CreateControlGrid()
	{
		Grid controlGrid = new Grid
		{
			Margin = new Thickness(10),
			Background = new SolidColorBrush(Colors.LightGray)
		};

		controlGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
		controlGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
		controlGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

		Button exitButton = new Button
		{
			Content = "Exit Game",
			Margin = new Thickness(5)
		};

		Button restartButton = new Button
		{
			Content = "Restart",
			Margin = new Thickness(5)
		};

		ComboBox difficultyComboBox = new ComboBox
		{
			Margin = new Thickness(5),
			ItemsSource = new[] { "Easy", "Medium", "Hard" },
			SelectedIndex = 0
		};

		Grid.SetColumn(exitButton, 0);
		Grid.SetColumn(restartButton, 1);
		Grid.SetColumn(difficultyComboBox, 2);

		controlGrid.Children.Add(exitButton);
		controlGrid.Children.Add(restartButton);
		controlGrid.Children.Add(difficultyComboBox);

		return controlGrid;
	}

	private Grid CreateYahtzeeGrid(bool isLeftPlayer)
	{
		Grid yahtzeeGrid = new Grid
		{
			Margin = new Thickness(10),
			Background = new SolidColorBrush(Colors.White)
		};

		int rows = 19;
		int columns = 3;
		for (int i = 0; i < rows; i++)
		{
			yahtzeeGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		}

		for (int j = 0; j < columns; j++)
		{
			yahtzeeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		}

		if (!isLeftPlayer)
		{
			Button rollDiceButton = new Button
			{
				Content = "Roll Dice",
				Margin = new Thickness(5),
				HorizontalAlignment = HorizontalAlignment.Center
			};
			rollDiceButton.Click += RollDiceButton_Click;
			Grid.SetRow(rollDiceButton, 0);
			Grid.SetColumn(rollDiceButton, 0);
			Grid.SetColumnSpan(rollDiceButton, 3);
			yahtzeeGrid.Children.Add(rollDiceButton);
		}

		AddHeader(yahtzeeGrid, "UPPER SECTION", 1, 0, 10);

		string[] upperSection =
		{
			"Aces = 1", "Twos = 2", "Threes = 3", "Fours = 4", "Fives = 5", "Sixes = 6", "UPPER TOTAL"
		};
		string[] upperSectionDescription =
		{
			"Count and Add Only Aces", "Count and Add Only Twos", "Count and Add Only Threes", "Count and Add Only Fours",
			"Count and Add Only Fives", "Count and Add Only Sixes", "<--"
		};
		for (int i = 0; i < upperSection.Length; i++)
		{
			AddCell(yahtzeeGrid, upperSection[i], 2 + i, isLeftPlayer ? 0 : 2, 1);
			AddCell(yahtzeeGrid, upperSectionDescription[i], 2 + i, 1, 1);
			if (isLeftPlayer)
			{
				AddCell(yahtzeeGrid, i < upperSection.Length - 1 ? "-" : "0", 2 + i, 2, 1, borderColor: Colors.White);
			}
			else
			{
				if (i < upperSection.Length - 1)
				{
					AddCellWithUseButton(yahtzeeGrid, i + 2, 0);
				}
				else
				{
					AddCell(yahtzeeGrid, "0", 2 + i, 0, 1, borderColor: Colors.White);
				}
			}
		}

		AddHeader(yahtzeeGrid, "LOWER SECTION", 9, 0, 10);
		string[] lowerSection =
		{
			"3 of a kind", "4 of a kind", "Full House", "Sm. Straight", "Lg. Straight", "YAHTZEE", "Chance", "LOWER TOTAL",
			"TOTAL"
		};
		string[] lowerSectionDescription =
		{
			"Add Total of All Dice", "Add Total of All Dice", "Score 25", "Score 30", "Score 40", "Score 50",
			"Add Total of All 5 Dice", "<--", "of upper and lower section"
		};
		for (int i = 0; i < lowerSection.Length; i++)
		{
			AddCell(yahtzeeGrid, lowerSection[i], 10 + i, isLeftPlayer ? 0 : 2, 1);
			AddCell(yahtzeeGrid, lowerSectionDescription[i], 10 + i, 1, 1);
			if (isLeftPlayer)
			{
				AddCell(yahtzeeGrid, i < lowerSection.Length - 2 ? "-" : "0", 10 + i, 2, 1, borderColor: Colors.White);
			}
			else
			{
				if (i < lowerSection.Length - 2)
				{
					AddCellWithUseButton(yahtzeeGrid, i + 10, 0);
				}
				else
				{
					AddCell(yahtzeeGrid, "0", 10 + i, 0, 1, borderColor: Colors.White);
				}
			}
		}

		return yahtzeeGrid;
	}

	private Grid CreateSquaresGrid(bool isLeftPlayer)
	{
		Grid squaresGrid = new Grid
		{
			Margin = new Thickness(10),
			Background = new SolidColorBrush(Colors.LightGray)
		};

		for (int i = 0; i < 1; i++)
		{
			squaresGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		}

		for (int j = 0; j < 5; j++)
		{
			squaresGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		}

		for (int j = 0; j < 5; j++)
		{
			Button square = new Button
			{
				Background = new SolidColorBrush(isLeftPlayer ? Colors.White : Colors.Yellow),
				BorderBrush = new SolidColorBrush(Colors.Black),
				BorderThickness = new Thickness(1),
				Margin = new Thickness(2),
				Tag = 0
			};
			if (isLeftPlayer)
			{
				square.Click += (s, e) => ToggleDiceSelectionBot();
			}
			else
			{
				square.Click += (s, e) => ToggleDiceSelection(square);
			}

			Grid.SetRow(square, 0);
			Grid.SetColumn(square, j);

			squaresGrid.Children.Add(square);

			if (isLeftPlayer)
			{
				leftDice[square] = RollSingleDice();
				UpdateSquare(square, leftDice);
			}
			else
			{
				rightDice[square] = RollSingleDice();
				UpdateSquare(square, rightDice);
			}
		}

		return squaresGrid;
	}

	private void ToggleDiceSelectionBot()
	{
	}

	private void RollDiceButton_Click(object sender, RoutedEventArgs e)
	{
		if (rollCount < 3)
		{
			foreach (var dice in rightDice)
			{
				Button square = dice.Key;
				SolidColorBrush currentBackground = square.Background as SolidColorBrush;

				if (currentBackground != null && currentBackground.Color == Colors.Yellow)
				{
					rightDice[square] = RollSingleDice();
					UpdateSquare(square, rightDice);
				}
			}

			rollCount++;
		}
		else
		{
			MessageBox.Show("Нізя більше кидати, потяг поїхав, обирай куди очки записувати, голова");
		}
	}

	private int RollSingleDice() => random.Next(1, 7);

	private void UpdateSquare(Button square, Dictionary<Button, int> diceDict)
	{
		int value = diceDict[square];
		square.Content = value.ToString();
	}

	private void ToggleDiceSelection(Button square)
	{
		SolidColorBrush currentBackground = square.Background as SolidColorBrush;

		if (currentBackground != null && currentBackground.Color == Colors.Yellow)
		{
			square.Background = new SolidColorBrush(Colors.White);
		}
		else
		{
			square.Background = new SolidColorBrush(Colors.Yellow);
		}
	}

	private void AddHeader(Grid grid, string text, int row, int column, int columnSpan)
	{
		TextBlock header = new TextBlock
		{
			Text = text,
			FontSize = 18,
			FontWeight = FontWeights.Bold,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Background = new SolidColorBrush(Colors.LightGray),
			Margin = new Thickness(2)
		};

		Grid.SetRow(header, row);
		Grid.SetColumn(header, column);
		Grid.SetColumnSpan(header, columnSpan);
		grid.Children.Add(header);
	}

	private void AddCell(Grid grid, string text, int row, int column, int columnSpan = 1, Color borderColor = default)
	{
		Border border = new Border
		{
			BorderBrush = new SolidColorBrush(borderColor == default ? Colors.Black : Colors.White),
			BorderThickness = new Thickness(1),
			Background = new SolidColorBrush(Colors.White)
		};

		TextBlock cell = new TextBlock
		{
			Text = text,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(2)
		};

		Grid.SetRow(border, row);
		Grid.SetColumn(border, column);
		Grid.SetColumnSpan(border, columnSpan);

		border.Child = cell;
		grid.Children.Add(border);
	}

	private void AddCellWithUseButton(Grid grid, int row, int column)
	{
		Button useButton = new Button
		{
			Content = "Use",
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(2)
		};
		useButton.Click += (s, e) => { CalculateScore(useButton); };

		Grid.SetRow(useButton, row);
		Grid.SetColumn(useButton, column);
		grid.Children.Add(useButton);
	}

	private void CalculateScore(Button useButton)
	{
		if (rollCount == 0)
		{
			MessageBox.Show("Що ти додати хочеш? Ти ще навіть кидку не зробив");
			return;
		}

		Grid parentGrid = useButton.Parent as Grid;
		if (parentGrid != null)
		{
			int row = Grid.GetRow(useButton);
			int column = Grid.GetColumn(useButton);

			int score = 0;

			switch (row)
			{
				case 2:
					foreach (var dice in rightDice)
					{
						if (dice.Value == 1)
							score += dice.Value;
					}

					break;

				case 3:
					foreach (var dice in rightDice)
					{
						if (dice.Value == 2)
							score += dice.Value;
					}

					break;

				case 4:
					foreach (var dice in rightDice)
					{
						if (dice.Value == 3)
							score += dice.Value;
					}

					break;

				case 5:
					foreach (var dice in rightDice)
					{
						if (dice.Value == 4)
							score += dice.Value;
					}

					break;

				case 6:
					foreach (var dice in rightDice)
					{
						if (dice.Value == 5)
							score += dice.Value;
					}

					break;

				case 7:
					foreach (var dice in rightDice)
					{
						if (dice.Value == 6)
							score += dice.Value;
					}

					break;

				case 10:
				{
					var groups = rightDice.Values.GroupBy(x => x)
						.Where(g => g.Count() >= 3);


					if (groups.Any())
					{
						score = rightDice.Values.Sum();
					}
					else
					{
						MessageBox.Show("Це НЕ комбінація 3, тому у кількість очок було записано 0.");
						score = 0;
					}

					break;
				}

				case 11:
				{
					var groups = rightDice.Values.GroupBy(x => x)
						.Where(g => g.Count() >= 4);

					if (groups.Any())
					{
						score = rightDice.Values.Sum();
					}
					else
					{
						MessageBox.Show("Це НЕ комбінація 4, тому у кількість очок було записано 0.");
						score = 0;
					}

					break;
				}

				case 12:
				{
					var groups = rightDice.Values.GroupBy(x => x);
					var threeOfAKind = groups.Any(g => g.Count() == 3);
					var twoOfAKind = groups.Any(g => g.Count() == 2);
					if (threeOfAKind && twoOfAKind)
					{
						score = 25;
					}
					else
					{
						MessageBox.Show("Тобі НЕ фортануло і це НЕ фул-Халус, тому у кількість очок було записано 0.");
						score = 0;
					}

					break;
				}

				case 13:
					if (rightDice.Values.OrderBy(x => x).SequenceEqual(new[] { 1, 2, 3, 4, 5 }))
					{
						score = 30;
					}
					else
					{
						MessageBox.Show("Це НЕ послідовність 5, тому у кількість очок було записано 0.");
						score = 0;
					}

					break;

				case 14:
					if (rightDice.Values.OrderBy(x => x).SequenceEqual(new[] { 2, 3, 4, 5, 6 }))
					{
						score = 40;
					}
					else
					{
						MessageBox.Show("Це НЕ послідовність 6, тому у кількість очок було записано 0.");
						score = 0;
					}

					break;

				case 15:
				{
					var groups = rightDice.Values.GroupBy(x => x)
						.Where(g => g.Count() == 5);

					if (groups.Any())
					{
						score = 50;
					}
					else
					{
						MessageBox.Show("Це НЕ ЯЦЗИИИИ, тому у кількість очок було записано 0.");
						score = 0;
					}

					break;
				}

				case 16:
					score = rightDice.Values.Sum();
					break;

				default:
					return;
			}

			rollCount = 0;

			parentGrid.Children.Remove(useButton);

			TextBlock scoreText = new TextBlock
			{
				Text = score.ToString(),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(2)
			};

			if (row < 9)
			{
				string currentValueText = GetCellValue(parentGrid, 8, 0);
				int upperTotal = int.Parse(currentValueText ?? "0");
				upperTotal += score;
				if (upperTotal >= 63)
					upperTotal += 35;

				RemoveCellContent(parentGrid, 8, 0);

				Border border = new Border
				{
					BorderBrush = new SolidColorBrush(Colors.White),
					BorderThickness = new Thickness(1),
					Background = new SolidColorBrush(Colors.White)
				};
				TextBlock updatedScoreText = new TextBlock
				{
					Text = upperTotal.ToString(),
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Margin = new Thickness(2)
				};
				border.Child = updatedScoreText;

				Grid.SetRow(border, 8);
				Grid.SetColumn(border, 0);
				parentGrid.Children.Add(border);
			}
			else
			{
				string currentValueText = GetCellValue(parentGrid, 17, 0);
				int lowerTotal = int.Parse(currentValueText ?? "0");
				lowerTotal += score;

				RemoveCellContent(parentGrid, 17, 0);

				Border border = new Border
				{
					BorderBrush = new SolidColorBrush(Colors.White),
					BorderThickness = new Thickness(1),
					Background = new SolidColorBrush(Colors.White)
				};
				TextBlock updatedScoreText = new TextBlock
				{
					Text = lowerTotal.ToString(),
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Margin = new Thickness(2)
				};
				border.Child = updatedScoreText;

				Grid.SetRow(border, 17);
				Grid.SetColumn(border, 0);
				parentGrid.Children.Add(border);
			}

			string currentGlobalTotal = GetCellValue(parentGrid, 18, 0);
			int globalTotal = int.Parse(currentGlobalTotal ?? "0");

			string upperTotalText = GetCellValue(parentGrid, 8, 0);
			int upperFinalTotal = int.Parse(upperTotalText ?? "0");

			string lowerTotalText = GetCellValue(parentGrid, 17, 0);
			int lowerFinalTotal = int.Parse(lowerTotalText ?? "0");

			globalTotal = upperFinalTotal + lowerFinalTotal;

			RemoveCellContent(parentGrid, 18, 0);

			Border borderTotal = new Border
			{
				BorderBrush = new SolidColorBrush(Colors.White),
				BorderThickness = new Thickness(1),
				Background = new SolidColorBrush(Colors.White)
			};
			TextBlock updatedTotalText = new TextBlock
			{
				Text = globalTotal.ToString(),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(2)
			};
			borderTotal.Child = updatedTotalText;

			Grid.SetRow(borderTotal, 18);
			Grid.SetColumn(borderTotal, 0);
			parentGrid.Children.Add(borderTotal);

			Grid.SetRow(scoreText, row);
			Grid.SetColumn(scoreText, column);
			parentGrid.Children.Add(scoreText);

			MiddleBotTurn();
		}
	}

	private List<TextBlock> AvailableBotOptions(Grid yahtzeeGrid)
	{
		List<TextBlock> textBlocksWithDash = new List<TextBlock>();

		foreach (var child in yahtzeeGrid.Children)
		{
			if (child is Border border && border.Child is TextBlock textBlock && textBlock.Text == "-")
			{
				textBlocksWithDash.Add(textBlock);
			}
		}

		return textBlocksWithDash;
	}

	private void EasyBotTurn()
	{
		for (int i = 0; i < random.Next(1, 4); i++)
		{
			foreach (var dice in leftDice)
			{
				Button square = dice.Key;
				leftDice[square] = RollSingleDice();
				UpdateSquare(square, leftDice);
			}
		}

		List<TextBlock> availableBotOptions = AvailableBotOptions(leftYahtzeeGrid);
		TextBlock chosenOption = availableBotOptions[random.Next(0, availableBotOptions.Count)];
		int row = Grid.GetRow(chosenOption.Parent as Border);
		int score = 0;

		switch (row)
		{
			case 2:
				foreach (var dice in leftDice)
				{
					if (dice.Value == 1)
						score += dice.Value;
				}

				break;

			case 3:
				foreach (var dice in leftDice)
				{
					if (dice.Value == 2)
						score += dice.Value;
				}

				break;

			case 4:
				foreach (var dice in leftDice)
				{
					if (dice.Value == 3)
						score += dice.Value;
				}

				break;

			case 5:
				foreach (var dice in leftDice)
				{
					if (dice.Value == 4)
						score += dice.Value;
				}

				break;

			case 6:
				foreach (var dice in leftDice)
				{
					if (dice.Value == 5)
						score += dice.Value;
				}

				break;

			case 7:
				foreach (var dice in leftDice)
				{
					if (dice.Value == 6)
						score += dice.Value;
				}

				break;

			case 10:
			{
				var groups = leftDice.Values.GroupBy(x => x)
					.Where(g => g.Count() >= 3);

				score = groups.Any() ? leftDice.Values.Sum() : 0;

				break;
			}

			case 11:
			{
				var groups = leftDice.Values.GroupBy(x => x)
					.Where(g => g.Count() >= 4);

				score = groups.Any() ? leftDice.Values.Sum() : 0;

				break;
			}

			case 12:
			{
				var groups = leftDice.Values.GroupBy(x => x);
				var threeOfAKind = groups.Any(g => g.Count() == 3);
				var twoOfAKind = groups.Any(g => g.Count() == 2);
				score = threeOfAKind && twoOfAKind ? 25 : 0;

				break;
			}

			case 13:
				score = leftDice.Values.OrderBy(x => x).SequenceEqual(new[] { 1, 2, 3, 4, 5 }) ? 30 : 0;

				break;

			case 14:
				score = leftDice.Values.OrderBy(x => x).SequenceEqual(new[] { 2, 3, 4, 5, 6 }) ? 40 : 0;

				break;

			case 15:
			{
				var groups = leftDice.Values.GroupBy(x => x)
					.Where(g => g.Count() == 5);

				score = groups.Any() ? 50 : 0;

				break;
			}

			case 16:
				score = leftDice.Values.Sum();
				break;

			default:
				return;
		}

		leftYahtzeeGrid.Children.Remove(chosenOption.Parent as Border);

		TextBlock scoreText = new TextBlock
		{
			Text = score.ToString(),
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(2)
		};

		if (row < 9)
		{
			string currentValueText = GetCellValue(leftYahtzeeGrid, 8, 2);
			int upperTotal = int.Parse(currentValueText ?? "0");
			upperTotal += score;
			if (upperTotal >= 63)
				upperTotal += 35;

			RemoveCellContent(leftYahtzeeGrid, 8, 2);

			Border border = new Border
			{
				BorderBrush = new SolidColorBrush(Colors.White),
				BorderThickness = new Thickness(1),
				Background = new SolidColorBrush(Colors.White)
			};
			TextBlock updatedScoreText = new TextBlock
			{
				Text = upperTotal.ToString(),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(2)
			};
			border.Child = updatedScoreText;

			Grid.SetRow(border, 8);
			Grid.SetColumn(border, 2);
			leftYahtzeeGrid.Children.Add(border);
		}
		else
		{
			string currentValueText = GetCellValue(leftYahtzeeGrid, 17, 2);
			int lowerTotal = int.Parse(currentValueText ?? "0");
			lowerTotal += score;

			RemoveCellContent(leftYahtzeeGrid, 17, 2);

			Border border = new Border
			{
				BorderBrush = new SolidColorBrush(Colors.White),
				BorderThickness = new Thickness(1),
				Background = new SolidColorBrush(Colors.White)
			};
			TextBlock updatedScoreText = new TextBlock
			{
				Text = lowerTotal.ToString(),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(2)
			};
			border.Child = updatedScoreText;

			Grid.SetRow(border, 17);
			Grid.SetColumn(border, 2);
			leftYahtzeeGrid.Children.Add(border);
		}

		string currentGlobalTotal = GetCellValue(leftYahtzeeGrid, 18, 2);
		int globalTotal = int.Parse(currentGlobalTotal ?? "0");

		string upperTotalText = GetCellValue(leftYahtzeeGrid, 8, 2);
		int upperFinalTotal = int.Parse(upperTotalText ?? "0");

		string lowerTotalText = GetCellValue(leftYahtzeeGrid, 17, 2);
		int lowerFinalTotal = int.Parse(lowerTotalText ?? "0");

		globalTotal = upperFinalTotal + lowerFinalTotal;

		RemoveCellContent(leftYahtzeeGrid, 18, 2);

		Border borderTotal = new Border
		{
			BorderBrush = new SolidColorBrush(Colors.White),
			BorderThickness = new Thickness(1),
			Background = new SolidColorBrush(Colors.White)
		};
		TextBlock updatedTotalText = new TextBlock
		{
			Text = globalTotal.ToString(),
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(2)
		};
		borderTotal.Child = updatedTotalText;

		Grid.SetRow(borderTotal, 18);
		Grid.SetColumn(borderTotal, 2);
		leftYahtzeeGrid.Children.Add(borderTotal);

		Grid.SetRow(scoreText, row);
		Grid.SetColumn(scoreText, 2);
		leftYahtzeeGrid.Children.Add(scoreText);
	}

	private void MiddleBotTurn()
	{
		for (int reroll = 0; reroll < random.Next(1, 3); reroll++)
		{
			var diceCounts = leftDice.Values.GroupBy(x => x)
				.ToDictionary(g => g.Key, g => g.Count());

			foreach (var dice in leftDice)
			{
				if (reroll == 0)
				{
					Button square = dice.Key;
					leftDice[square] = RollSingleDice();
					UpdateSquare(square, leftDice);
				}
				else if (ShouldReroll(dice.Value, diceCounts))
				{
					Button square = dice.Key;
					leftDice[square] = RollSingleDice();
					UpdateSquare(square, leftDice);
				}
			}
		}

		List<TextBlock> availableBotOptions = AvailableBotOptions(leftYahtzeeGrid);
		List<(int row, int score)> scoredOptions = CalculateScores(availableBotOptions);

		var bestOption = scoredOptions.OrderByDescending(option => option.score).First();
		ApplyScoreToCategory(bestOption.row, bestOption.score);
	}

	private bool ShouldReroll(int value, Dictionary<int, int> diceCounts)
	{
		return diceCounts[value] < 2;
	}

	private List<(int row, int score)> CalculateScores(List<TextBlock> availableOptions)
	{
		List<(int row, int score)> scoredOptions = new();
		foreach (var option in availableOptions)
		{
			int row = Grid.GetRow(option.Parent as Border);
			int score = CalculateScoreForRow(row);
			scoredOptions.Add((row, score));
		}

		return scoredOptions;
	}

	private int CalculateScoreForRow(int row)
	{
		var diceValues = leftDice.Values;
		switch (row)
		{
			case 2: return diceValues.Where(x => x == 1).Sum();
			case 3: return diceValues.Where(x => x == 2).Sum();
			case 4: return diceValues.Where(x => x == 3).Sum();
			case 5: return diceValues.Where(x => x == 4).Sum();
			case 6: return diceValues.Where(x => x == 5).Sum();
			case 7: return diceValues.Where(x => x == 6).Sum();
			case 10: return diceValues.GroupBy(x => x).Any(g => g.Count() >= 3) ? diceValues.Sum() : 0;
			case 11: return diceValues.GroupBy(x => x).Any(g => g.Count() >= 4) ? diceValues.Sum() : 0;
			case 12:
				var groups = diceValues.GroupBy(x => x);
				return groups.Any(g => g.Count() == 3) && groups.Any(g => g.Count() == 2) ? 25 : 0;
			case 13: return IsSmallStraight(diceValues) ? 30 : 0;
			case 14: return IsLargeStraight(diceValues) ? 40 : 0;
			case 15: return diceValues.GroupBy(x => x).Any(g => g.Count() == 5) ? 50 : 0;
			case 16: return diceValues.Sum();
			default: return 0;
		}
	}

	private bool IsSmallStraight(IEnumerable<int> diceValues)
	{
		var unique = new HashSet<int>(diceValues);
		return unique.SetEquals(new[] { 1, 2, 3, 4 }) || unique.SetEquals(new[] { 2, 3, 4, 5 }) ||
		       unique.SetEquals(new[] { 3, 4, 5, 6 });
	}

	private bool IsLargeStraight(IEnumerable<int> diceValues)
	{
		var unique = new HashSet<int>(diceValues);
		return unique.SetEquals(new[] { 1, 2, 3, 4, 5 }) || unique.SetEquals(new[] { 2, 3, 4, 5, 6 });
	}

	private void ApplyScoreToCategory(int row, int score)
	{
		Border existingBorder = leftYahtzeeGrid.Children
			.OfType<Border>()
			.FirstOrDefault(b => Grid.GetRow(b) == row && Grid.GetColumn(b) == 2);

		if (existingBorder != null)
		{
			leftYahtzeeGrid.Children.Remove(existingBorder);
		}

		Border newBorder = new Border
		{
			BorderBrush = new SolidColorBrush(Colors.White),
			BorderThickness = new Thickness(1),
			Background = new SolidColorBrush(Colors.White)
		};

		TextBlock scoreText = new TextBlock
		{
			Text = score.ToString(),
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(2)
		};
		newBorder.Child = scoreText;

		Grid.SetRow(newBorder, row);
		Grid.SetColumn(newBorder, 2);
		leftYahtzeeGrid.Children.Add(newBorder);

		if (row < 9)
		{
			string currentValueText = GetCellValue(leftYahtzeeGrid, 8, 2);
			int upperTotal = int.Parse(currentValueText ?? "0");
			upperTotal += score;
			if (upperTotal >= 63)
				upperTotal += 35;

			RemoveCellContent(leftYahtzeeGrid, 8, 2);

			Border border = new Border
			{
				BorderBrush = new SolidColorBrush(Colors.White),
				BorderThickness = new Thickness(1),
				Background = new SolidColorBrush(Colors.White)
			};
			TextBlock updatedScoreText = new TextBlock
			{
				Text = upperTotal.ToString(),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(2)
			};
			border.Child = updatedScoreText;

			Grid.SetRow(border, 8);
			Grid.SetColumn(border, 2);
			leftYahtzeeGrid.Children.Add(border);
		}
		else
		{
			string currentValueText = GetCellValue(leftYahtzeeGrid, 17, 2);
			int lowerTotal = int.Parse(currentValueText ?? "0");
			lowerTotal += score;

			RemoveCellContent(leftYahtzeeGrid, 17, 2);

			Border border = new Border
			{
				BorderBrush = new SolidColorBrush(Colors.White),
				BorderThickness = new Thickness(1),
				Background = new SolidColorBrush(Colors.White)
			};
			TextBlock updatedScoreText = new TextBlock
			{
				Text = lowerTotal.ToString(),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(2)
			};
			border.Child = updatedScoreText;

			Grid.SetRow(border, 17);
			Grid.SetColumn(border, 2);
			leftYahtzeeGrid.Children.Add(border);
		}

		UpdateTotalScore();
	}

	private void UpdateTotalScore()
	{
		string upperTotalText = GetCellValue(leftYahtzeeGrid, 8, 2);
		int upperTotal = int.Parse(upperTotalText ?? "0");

		string lowerTotalText = GetCellValue(leftYahtzeeGrid, 17, 2);
		int lowerTotal = int.Parse(lowerTotalText ?? "0");

		int globalTotal = upperTotal + lowerTotal;

		RemoveCellContent(leftYahtzeeGrid, 18, 2);

		Border borderTotal = new Border
		{
			BorderBrush = new SolidColorBrush(Colors.White),
			BorderThickness = new Thickness(1),
			Background = new SolidColorBrush(Colors.White)
		};

		TextBlock updatedTotalText = new TextBlock
		{
			Text = globalTotal.ToString(),
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(2)
		};
		borderTotal.Child = updatedTotalText;

		Grid.SetRow(borderTotal, 18);
		Grid.SetColumn(borderTotal, 2);
		leftYahtzeeGrid.Children.Add(borderTotal);
	}

	private string? GetCellValue(Grid grid, int row, int column)
	{
		foreach (var child in grid.Children)
		{
			if (child is Border border &&
			    Grid.GetRow(border) == row &&
			    Grid.GetColumn(border) == column &&
			    border.Child is TextBlock textBlock)
			{
				return textBlock.Text;
			}
		}

		return null;
	}

	private void RemoveCellContent(Grid grid, int row, int column)
	{
		UIElement elementToRemove = null;

		foreach (var child in grid.Children)
		{
			if (Grid.GetRow((UIElement)child) == row && Grid.GetColumn((UIElement)child) == column)
			{
				elementToRemove = (UIElement)child;
				break;
			}
		}

		if (elementToRemove != null)
		{
			grid.Children.Remove(elementToRemove);
		}
	}
}