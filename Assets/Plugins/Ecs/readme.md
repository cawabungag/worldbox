

**in development!**

## Измененная библиотека EcsLite

Данная версия не совместима по api с кодом, написанным для обычной EcsLite. 

### Get vs GetRef
Самое большое отличие от оригинальной версии, которое нужно учесть - это возврат компонента из пула по ссылке или по значению. Пример:

	var pool = world.GetPool<PositionComponent>();
	
	//запрос компонента на чтение без возможности его модифицирования
	Vector3 pos = pool.Get(entity).value;
	
	//запрос компонента на изменение
	ref Vector3 pos = ref pool.GetRef(entity).value;
	pos.x = 0;
	
	//или так
	ref PositionComponent component = ref pool.GetRef(entity);
	component.value.x = 0;
	
	//или так
    poolPosition.GetRef(entity).value.x = 0;

Данное изменение позволяет реализовать EventSystem и подписку на изменение компонентов у Entity, похожее на **Entitas** фреймворк. [Смотри Listeners Test.](https://bitbucket.org/fabros_team/module-ecs/src/master/UnitySample/Assets/Tests/Runtime/EventsTest.cs)

#### Reactive запросы
Примеры "реактивных" запросов, которые можно делать к EcsWorld:

	public class MyReactiveSystem : IEcsRunSystem  
	{  
	  public void Run(EcsSystems systems)  
	  {
	      var world = systems.GetWorld(); 
	      
          //Дай мне фильтр на все сущности, у которых изменилась позиция за последний systems.Run()
          var filter1 = world.Filter<PositionComponent>().IncChanges<PositionComponent>().End();

          //Дай мне фильтр на все сущности, у которых добавились PositionComponent за последний systems.Run()
          var filter2 = world.FilterAdded<PositionComponent>().End();

	      //Дай мне фильтр на все сущности, у которых удален PositionComponent за последний systems.Run()
          var filter3 = world.FilterRemoved<PositionComponent>().End();
          
		  //Чуть сложнее
		  //Дай мне фильтр на все сущности, у которых есть RotationComponent и был добавлен PositionComponent за последний systems.Run()	
          var filter4 = world.Filter<RotationComponent>().IncAdded<PositionComponent>.End();

		  /*  Дай мне фильтр на все сущности, у которых есть PositionComponent, SpeedComponent 
		      и был добавлен RotationComponent за последний systems.Run()	
		  */
          var filter5 = world.Filter<PositionComponent>()
                             .Inc<SpeedComponent>()
                             .IncAdded<RotationComponent>.End();
      }
     }


Чтоб эти запросы работали правильно EventsSystem должна быть добавлена в список исполняемых систем (даже если не используется подписка на события) и находиться **после** MyReactiveSystem:

    var systems = new EcsSystems(world);

	//основные системы игры...
    systems.Add(new MyReactiveSystem());

	//EventSystems обычно располагаются в самом конце списка систем
	systems.Add(new EventsSystem<PositionComponent>());
	systems.Add(new EventsSystem<SomeOtherComponent>());
	systems.Add(new EventsSystem<HpComponent>());


### Replace
Добавлен метод Replace в пул, который *удалит* старый компонент, если он есть и добавит новый:

	pool.Replace(entity, new PositionComponent {value = new Vector3(0,0,0)});


### GetOrCreate
Добавлен метод GetOrCreateRef в пул, который вернет существующий компонент или создаст новый:

	pool.GetOrCreateRef(entity).value = new Vector3(0,0,0);	

### Разница между Replace и GetOrCreate
Replace заствляет передать в себя целиком новый уже настроенный компонент, а GetOrCreate удобен там где-надо поменять одно поле из нескольких, сохранив все остальные поля прежними.
Для EventSystems и Reactive запросов это тоже имеет значение.


### Any Listeners

Подписка на глобальный эвент изменения или удаления компонента:

	var listener = world.CreateAnyListener();
    listener.SetAnyChangedListener<ObjectiveOpenedComponent>(this);
    listener.SetAnyRemovedListener<ObjectiveCompletedComponent>(this);

Работает в паре с EventsSystem.

### Self Listeners

Подписка на изменение компонентов одной сущности:

	int entity = world.NewEntity();
	entity.AddChangedListener<HpComponent>(this);
	entity.AddRemovedListener<HpComponent>(this);

Работает в паре с EventsSystem.


### Unique компоненты

В EcsWorld добавлены Unique компоненты, позволяющие упростить код, там где предполагается наличие только одного компонента на весь мир. Пример:

	world.AddUnique<TickComponent>.value = 1;
	world.GetUniqueRef<TickComponent>().value = 2;
	var tick = world.GetUnique<TickComponent>().value;
	//и прочие по аналогии с pool
	

### Пустые компоненты
Для пустых компонтов без своих полей добавлен аттрибут **EmptyComponent**:
		
	[EmptyComponent]
	struct MyEmptyComponent {}

Он позволяет экономить память выделяемую внутри EcsPool и немного ускоряет работу с ним.



### Отключено самоуничтожение entity

В оригинальной версии при удалении последнего компонента с entity она автоматически удалялась и становилась невалидной:


    var world = systems.GetWorld();  
      
    var entity = world.NewEntity();  
    var pool = world.GetPool<PositionComponent>();  
    pool.Add(entity).value = new Vector3(0, 0, 0);  
    pool.Del(entity);  
    //Раньше тут выбросилось бы исключение, потому что entity больше не существует
    var hasPosition = pool.Has(entity); 

Сейчас же, каждый созданный вами entity должен удаляться вручную с помощью метода DelEntity:

		
	world.DelEntity(entity);


### GetNullable

Вместо конструкций 'if Has then Get' иногда удобнее писать в одну строчку используя метод GetNullable:

	var speed = world.GetPool<SpeedComponent>().GetNullable(entity)?.Speed ?? 1f;


### TryGet

Вместо конструкций 'if Has then Get' иногда удобнее писать используя TryGet:

	if (world.GetPool<SpeedComponent>().TryGet(entity, out SpeedComponent speed))
	{
		Debug.Log(speed.Value);
	}


### Прочие изменения

- некоторые правки для возможности быстро переключать миры без лишних нагрузок на GC


### module-ecs/UnitySample

Пустой Unity проект, в котором лежат только unit tests для новой функциональности в Ecs.
