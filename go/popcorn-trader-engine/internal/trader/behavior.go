package trader

type Behavior interface {
	Analyze() float64
	Trade()
}
