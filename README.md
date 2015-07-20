# khipu

[![Build Status](https://travis-ci.org/khipu/lib-dotnet.png)](https://travis-ci.org/khipu/lib-dotnet)

[khipu - Yo pago, Yo cobro](https://khipu.com)

Libretia .Net para utilizar los servicios de Khipu.com

Versión API Khipu: 1.3

La documentación de Khipu.com se puede ver desde aquí: [https://khipu.com/page/api](https://khipu.com/page/api)

## Uso

Primero bajar y compilar el código fuente o usar la librería desde nuget http://nuget.org.
Para instalar con nuget:

  Install-Package KhipuClient

Luego debes importar el namespace Khipu.Api en el archivo respectivo.
En c# la importacion se hace mediante "using Khipu.Api;"

## Operaciones

Esta biblioteca implementa los siguientes servicios de khipu:

1. Obtener listado de bancos
2. Crear cobros y enviarlos por mail. 
3. Crear una página de Pago.
4. Crear una página de Pago (autenticado).
5. Crear un pago y obtener su URL.
6. Crear un pago y obtener su URL (autenticado).
7. Recibir y validar la notificación de un pago.
  7.1 Recibir y validar la notificación de un pago (API de notificación 1.3 o superior)
  7.2 Recibir y validar la notificación de un pago (API de notificación 1.2 o inferior)
8. Verificar el estado de una cuenta de cobro.
9. Verificar el estado de un pago.
10. Marcar un pago como pagado.
11. Marcar un cobro como expirado.
12. Marcar un pago como rechazado.


### 1) Obtener listado de bancos.

Este servicio entrega un mapa con el listado de los bancos disponibles para efectuar un pago a un cobrador determinado.
Cada banco tiene su identificador, un nombre, el monto mínimo que se puede transferir desde él y un mensaje con
información importante.

```c#
    { 
       try{
          List<Dictionary<string, object>> banks = KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, 'SECRET_DEL_COBRADOR').ReceiverBanks();
          // A continuación utilizamos la información en nuestro proceso.
          // En este ejemplo se escribe la información a la salida estándar.
          Console.Out.WriteLine("Banks number:"+banks.Count);
		  foreach(Dictionary<string,object> item in banks){
				Console.Out.WriteLine (item["id"]);
				Console.Out.WriteLine (item["name"]);
				Console.Out.WriteLine (item["message"]);
				Console.Out.WriteLine (item["min-amount"]);
			}
       }catch(ApiException exception){
		 //Aqui hacemos el manejo de la excepcion
		 Console.Out.WriteLine(exception.ErrorType);
		 Console.Out.WriteLine(exception.Message);
       }
    }
```

### 2) Crear cobros y enviarlos por mail.

Este servicio entrega un mapa que contiene el identificador del cobro generado así como una lista de los pagos asociados
a este cobro. Por cada pago se tiene el ID, el correo asociado y la URL en khipu para pagar.

```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
		    KhipuResponse response = service.CreateEmail(new Dictionary<string, object>{
				{"subject", "Un cobro desde .Net"},
				{"pay_directly", true},
				{"send_emails", true},
				{"destinataries", "[ {\"name\": \"Juan Rojo\", \"email\": \"juan.rojo@ejemplo.com\", \"amount\": \"1000\"}, {\"name\": \"Pedro Piedra\", \"email\": \"pedro.piedra@ejemplo.com\", \"amount\": \"1000\"}, { \"name\": \"Ana Soto\", \"email\": \"ana.soto@ejemplo.com\", \"amount\": \"1000\"}]"} 
			});
          // A continuación utilizamos la información en nuestro proceso.
          // En este ejemplo se escribe la información a la salida estándar.

            Console.Out.WriteLine("Id de cobro:"+response["id"]);
            Console.Out.WriteLine(response.list("payments").Count+" cobros");
            foreach(KhipuResponse item in response.list("payments")){
            	Console.Out.WriteLine(item["id"]);
            	Console.Out.WriteLine(item["url"]);
            	foreach(KhipuResponse destinatarie in item.list("destinataries")){
            		Console.Out.WriteLine(destinatarie["name"]);
            		Console.Out.WriteLine(destinatarie["email"]);
            	}
            }

		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```
*Importante*: El parámetro "destinataries" se puede pasar como string Json o como una lista que internamente se transformara a su representacion Json.
    Un ejemplo válido de utilización de una lista como parámetro destinataries es:

			List<Dictionary<string,string>> destinataries = new List<Dictionary<string, string>> () {
				new Dictionary<string,string> () {
					{"name", "Juan Rojo"},
					{"email", "juan.rojo@ejemplo.com"},
					{"amount", "1000"}
				},
				new Dictionary<string,string> () {
					{"name", "Pedro Piedra"},
					{"email", "pedro.piedra@ejemplo.com"},
					{"amount", "1000"}
				},
				new Dictionary<string,string> () {
					{"name", "Ana Soto"},
					{"email", "ana.soto@ejemplo.com"},
					{"amount", "1000"}
				}
			};
			KhipuResponse response = service.CreateEmail(new Dictionary<string, object>{
				{"subject", "Un cobro desde Ruby"},
				{"pay_directly", true},
				{"send_emails", true},
				{"destinataries", destinataries } 
			});


### 3) Crear una página de Pago.

Este ejemplo genera un archivo .html con un formulario de pago en khipu.

```c#

	using (StreamWriter file = new StreamWriter("form.html"))
    {
	    KhipuApiEndPoint service =  KhipuApi.CreateHtmlHelper(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
		string form = endPoint.CreatePaymentForm (new Dictionary<string, object> {
			{"subject","Un cobro desde .Net"},
			{"body", "El cuerpo del cobro"},
			{"amount","1000"},
			{"email", "john.doe@gmail.com"}
		});
    	file.Write(sb);
    }

```

### 4) Crear una página de Pago (autenticado).

Para crear una página de pago que solo permita pagar usando una cuenta asociada a un RUT en particular debes usar el
mismo servicio del punto anterior indicando el RUT en el parámetro _payer_username_

```c#

	using (StreamWriter file = new StreamWriter("form.html"))
    {
	    KhipuApiEndPoint service =  KhipuApi.CreateHtmlHelper(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
		string form = endPoint.CreatePaymentForm (new Dictionary<string, object> {
			{"subject","Un cobro desde .Net"},
			{"body", "El cuerpo del cobro"},
			{"amount","1000"},
			{"email", "john.doe@gmail.com"},
			{"payer_username", "128723463" }
		});
    	file.Write(sb);
    }

```

### 5) Crear un pago y obtener su URL.

Este servicio entrega un mapa que contiene el identificador de un pago generado, su URL en khipu y la URL para iniciar
el pago desde un dispositivo móvil.

```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
			KhipuResponse resp = service.CreatePaymentUrl(new Dictionary<string, object>{
				{"subject","Un cobro desde .Net"},
				{"body", "El cuerpo del cobro"},
				{"amount","1000"},
				{"email", "john.doe@gmail.com"}
			});
		  // A continuación utilizamos la información en nuestro proceso.
          // En este ejemplo se escribe la información a la salida estándar.
			Console.Out.WriteLine(resp ["id"]);
			Console.Out.WriteLine(resp ["bill-id"]);
			Console.Out.WriteLine(resp ["url"]);
			Console.Out.WriteLine(resp ["manual-url"]);
			Console.Out.WriteLine(resp ["mobile-url"]);
			Console.Out.WriteLine(resp ["ready-for-terminal"]);
		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```

### 6) Crear un pago y obtener su URL (autenticado).

Este servicio es idéntico al anterior pero usando el parámetro _payer_username_ se fuerza que la cuenta corriente usada
para pagar debe estar asociada al RUT indicado.

```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
			KhipuResponse resp = service.CreatePaymentUrl(new Dictionary<string, object>{
				{"subject","Un cobro desde .Net"},
				{"body", "El cuerpo del cobro"},
				{"amount","1000"},
				{"email", "john.doe@gmail.com"},
				{"payer_username", "128723463"}
			});
		  // A continuación utilizamos la información en nuestro proceso.
          // En este ejemplo se escribe la información a la salida estándar.
			Console.Out.WriteLine(resp ["id"]);
			Console.Out.WriteLine(resp ["bill-id"]);
			Console.Out.WriteLine(resp ["url"]);
			Console.Out.WriteLine(resp ["manual-url"]);
			Console.Out.WriteLine(resp ["mobile-url"]);
			Console.Out.WriteLine(resp ["ready-for-terminal"]);
		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```


### 7) Validar la notificación de un pago.
### 7.1) Validar la notificación de un pago (API de notificación 1.3 o superior)
Este ejemplo contacta a khipu para obtener la notificación de un pago a partir de un token de notificación.
El resultado contiene el _receiver_id_, _transaction_id_, _amount_, _currency_, etc con lo que se debe validar el pago contra el backend.
En este ejemplo los parámetros se configuran a mano, pero en producción los datos deben obtenerse desde el request _request html_.

```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
			KhipuResponse resp = service.GetPaymentNotification(new Dictionary<string, object>{
                {"notification_token", "j8kPBHaPNy3PkCh...hhLvQbenpGjA"}
			});
		  // A continuación utilizamos la información en nuestro proceso.
          // En este ejemplo se escribe la información a la salida estándar.
			Console.Out.WriteLine(resp["notification_token"]);
			Console.Out.WriteLine(resp["receiver_id"]);
			Console.Out.WriteLine(resp["subject"]);
			Console.Out.WriteLine(resp["amount"]);
			Console.Out.WriteLine(resp["custom"]);
			Console.Out.WriteLine(resp["transaction_id"]);
			Console.Out.WriteLine(resp["payment_id"]);
			Console.Out.WriteLine(resp["currency"]);
			Console.Out.WriteLine(resp["payer_email"]);
		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```

En resp queda un hash con los valores de la notificación:

```c#
    {
        "notification_token"=>"j8kPBHaPNy3PkCh...hhLvQbenpGjA",
        "receiver_id"=>ID_DEL_COBRADOR,
        "subject"=>"Motivo del cobro",
        "amount"=>"100",
        "custom"=>"",
        "transaction_id"=>"MTX_123123",
        "payment_id"=>"qpclzun1nlej",
        "currency"=>"CLP",
        "payer_email"=>"ejemplo@gmail.com"
    }
``````

### 7.2) Validar la notificación de un pago (API de notificación 1.2 o inferior)
Este ejemplo contacta a khipu para validar los datos de una transacción. Para usar
este servicio no es necesario configurar el SECRET del cobrador. Se retorna true si la información del la notificación
es válida. En este ejemplo los parámetros se configuran a mano, pero en producción los datos deben obtenerse desde
el _request html_.

```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
			bool verified = service.VerifyPaymentNotification(new Dictionary<string, object>{
				{"api_version", "1.2"},
				{"notification_id", "aq1td2jl2uen"},
				{"subject", "Motivo de prueba"},
				{"amount", 12575},
                {"currency", "CLP"}, 
                {"transaction_id", "FTEEE5SWWO"}, 
                {"payer_email", "john.doe@gmail.com"},
                {"custom", "Custom info"}, 
                {"notification_signature", "j8kPBHaPNy3PkCh...hhLvQbenpGjA=="}
			});
		  // A continuación utilizamos la información en nuestro proceso.
          // En este ejemplo se escribe la información a la salida estándar.
			Console.Out.WriteLine(verified);
		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```

### 8) Verificar el estado de una cuenta de cobro.

Este servicio permite consultar el estado de una cuenta khipu. Se devuelve un mapa que indica si esta cuenta está
habilitada para cobrar y el tipo de cuenta (desarrollo o producción).

```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
			KhipuResponse resp = service.ReceiverStatus();
		  // A continuación utilizamos la información en nuestro proceso.
          // En este ejemplo se escribe la información a la salida estándar.
			Console.Out.WriteLine(resp["ready_to_collect"]);
			Console.Out.WriteLine(resp["type"]);
		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```
 
### 9) Verificar el estado de un pago.

Este servició sirve para verificar el estado de un pago.

```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
			KhipuResponse resp = service.PaymentStatus();
		  // A continuación utilizamos la información en nuestro proceso.
          // En este ejemplo se escribe la información a la salida estándar.
			Console.Out.WriteLine(resp["status"]);
			Console.Out.WriteLine(resp["detail"]);
		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```

### 10) Marcar un cobro como pagado.

Este servicio permite marcar un cobro como pagado. Si el pagador paga por un método alternativo a khipu, el cobrador
puede marcar este cobro como saldado.

```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
			service.SetPaidByReceiver(new Dictionary<string, object>{
				{"payment_id","54dhfsch6avd"}
			});
		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```

### 11) Marcar un cobro como expirado.

Este servicio permite adelantar la expiración del cobro. Se puede adjuntar un texto que será desplegado a la gente que
trate de ir a pagar.

```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
			service.SetBillExpired(new Dictionary<string, object>{
				{"bill_id","udmEe"},
				{"text","Plazo vencido"}
			});
		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```

### 12) Marcar un cobro como rechazado.

Este servicio permite rechazar pago con el fin de inhabilitarlo. Permite indicar la razón por la que el pagador rechaza
saldar este pago:


```c#
    {
        try{
            KhipuApiEndPoint service =  KhipuApi.CreateKhipuApi(ID_DEL_COBRADOR, SECRET_DEL_COBRADOR);
			service.SetRejectedByPayer(new Dictionary<string, object>{
				{"payment_id","0pk7xfgocry4"},
				{"text","El pago no corresponde"}
			});
		}catch(ApiException exception){
		  //Aqui hacemos el manejo de la excepcion
		  Console.Out.WriteLine(exception.ErrorType);
		  Console.Out.WriteLine(exception.Message);
       }

    }
```
