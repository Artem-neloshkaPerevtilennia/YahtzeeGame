using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yahtzee
{
	public partial class MainWindow : Window
	{
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
			Grid leftYahtzeeGrid = CreateYahtzeeGrid(true);
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
				ItemsSource = new string[] { "Easy", "Medium", "Hard" },
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

			int rows = 17;
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
				"Aces = 1", "Twos = 2", "Threes = 3", "Fours = 4", "Fives = 5", "Sixes = 6", "TOTAL SCORE", "BONUS", "TOTAL"
			};
			for (int i = 0; i < upperSection.Length; i++)
			{
				AddCell(yahtzeeGrid, upperSection[i], 2 + i, isLeftPlayer ? 0 : 2, 1);
				AddCell(yahtzeeGrid, upperSection[i], 2 + i, 1, 1);
				AddCellWithUseButton(yahtzeeGrid, i + 2, isLeftPlayer ? 2 : 0);
			}

			AddHeader(yahtzeeGrid, "LOWER SECTION", 11, 0, 10);
			string[] lowerSection =
			{
				"3 of a kind", "4 of a kind", "Full House", "Sm. Straight", "Lg. Straight", "YAHTZEE", "Chance",
				"YAHTZEE BONUS", "TOTAL"
			};
			for (int i = 0; i < lowerSection.Length; i++)
			{
				AddCell(yahtzeeGrid, lowerSection[i], 12 + i, isLeftPlayer ? 0 : 2, 1);
				AddCell(yahtzeeGrid, lowerSection[i], 12 + i, 1, 1);
				AddCellWithUseButton(yahtzeeGrid, 12 + i, isLeftPlayer ? 2 : 0);
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

			for (int i = 0; i < 1; i++) // Один рядок
			{
				squaresGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			}

			for (int j = 0; j < 5; j++) // П'ять колонок
			{
				squaresGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			}

			for (int j = 0; j < 5; j++)
			{
				Button square = new Button
				{
					Background = new SolidColorBrush(Colors.White),
					BorderBrush = new SolidColorBrush(Colors.Black),
					BorderThickness = new Thickness(1),
					Margin = new Thickness(2),
					Tag = 0
				};
				square.Click += (s, e) => ToggleDiceSelection(square);

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

		private void RollDiceButton_Click(object sender, RoutedEventArgs e)
		{
			if (rollCount < 3)
			{
				foreach (var dice in rightDice)
				{
					Button square = dice.Key;
					SolidColorBrush currentBackground = square.Background as SolidColorBrush;

					// Кидаємо тільки вибрані кубики
					if (currentBackground != null && currentBackground.Color == Colors.Yellow)
					{
						rightDice[square] = RollSingleDice();
						UpdateSquare(square, rightDice);
					}
				}

				rollCount++;

				if (rollCount == 3)
				{
					MessageBox.Show("Ваш хід закінчився! Перехід до бота.");
					rollCount = 0;
					BotTurn();
				}
			}
		}

		private void BotTurn()
		{
			// Логіка ходу бота (порожня поки що)
			MessageBox.Show("Хід бота завершено. Ваш хід!");
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

		private void AddCell(Grid grid, string text, int row, int column, int columnSpan = 1)
		{
			Border border = new Border
			{
				BorderBrush = new SolidColorBrush(Colors.Black),
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
			useButton.Click += (s, e) => CalculateScore(useButton);

			Grid.SetRow(useButton, row);
			Grid.SetColumn(useButton, column);
			grid.Children.Add(useButton);
		}

		private void CalculateScore(Button useButton)
		{
			// Порожня функція для обчислення очок
		}
	}
}
