# Profitron

Profitron is the trader engine for the popcorn market. It needs to be able to act fast based on the scores of the analysts (of the analyst engine in the oracle engine)
and make trades based on those analyst scores.

To achieve this we need to have a trader pool, a trader pool manager that spins up and manages the trader pool.

A trader consists of a state, we know the following states:

- Idle
- Fetching
- Analyzing
- Trading

These states live in a loop. The data of the trader will contain the current state and based on the current state move on to the next and perform its action. Besides
the states the trader should also have a configuration, the configuration tells the trader how it should react, is it an aggressive trader or is it a more long term trader.

To determine if a trader should trade we should first acquire the data of a trader this means that we would need to do the following:

- Manage a watch list (this is done via the broker)
- Get the portfolio
- Get the analyst score
- Place a trade
