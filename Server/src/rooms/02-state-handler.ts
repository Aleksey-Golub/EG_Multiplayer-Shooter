import { Room, Client } from "colyseus";
import { Schema, type, MapSchema } from "@colyseus/schema";

export class Player extends Schema {
    // uint8 is byte in c#
    @type("uint8")
    loss = 0;
    
    // int8 is sbyte in c#
    @type("int8")
    maxHp = 0;
    @type("int8")
    currentHp = 0;

    // max client speed
    @type("number")
    speed = 0;

    // p - position
    @type("number")
    pX = Math.floor(Math.random() * 50) - 25;
    @type("number")
    pY = 0;
    @type("number")
    pZ = Math.floor(Math.random() * 50) - 25;
    
    // v - velocity
    @type("number")
    vX = 0;
    @type("number")
    vY = 0;
    @type("number")
    vZ = 0;

    // r - eulerAngle rotation
    @type("number")
    rX = 0;
    @type("number")
    rY = 0;
}

export class State extends Schema {
    @type({ map: Player })
    players = new MapSchema<Player>();

    something = "This attribute won't be sent to the client-side";

    createPlayer(sessionId: string, options: any) {
        const player = new Player();
        player.speed = options.speed;
        player.maxHp = options.maxHp;
        player.currentHp = options.maxHp;

        this.players.set(sessionId, player);
    }

    removePlayer(sessionId: string) {
        this.players.delete(sessionId);
    }

    movePlayer (sessionId: string, data: any) {
        const player = this.players.get(sessionId);
        
        player.pX = data.pX;
        player.pY = data.pY;
        player.pZ = data.pZ;

        player.vX = data.vX;
        player.vY = data.vY;
        player.vZ = data.vZ;

        player.rX = data.rX;
        player.rY = data.rY;
    }
}

export class StateHandlerRoom extends Room<State> {
    maxClients = 2;

    onCreate (options) {
        console.log("StateHandlerRoom created!", options);

        this.setState(new State());

        this.onMessage("move", (client, data) => {
            console.log("StateHandlerRoom received `move` message from", client.sessionId, ":", data);
            this.state.movePlayer(client.sessionId, data);
        });

        this.onMessage("shoot", (client, data) => {
            console.log("StateHandlerRoom received `shoot` message from", client.sessionId, ":", data);
            // send new message to all clents, except `client`
            this.broadcast("Shoot", data, {except: client});
        });

        this.onMessage("damage", (client, data) => {
            console.log("StateHandlerRoom received `damage` message from", client.sessionId, ":", data);
            // send new message to all clents, except `client`
            const damagedClientId = data.id;
            const player = this.state.players.get(damagedClientId);
            
            let hp = player.currentHp - data.appliedDamage;

            if (hp > 0){
                player.currentHp = hp;
                return;
            }
            else{
                player.loss++;
                player.currentHp = player.maxHp;

                const x = Math.floor(Math.random() * 50) - 25;
                const z = Math.floor(Math.random() * 50) - 25;
            
                for (let i = 0; i < this.clients.length; i++) {
                    if(this.clients[i].id == damagedClientId){

                        const message = JSON.stringify({x, z});
                        this.clients[i].send("Restart", message);
                    }                    
                }
            }
        });
    }

    onAuth(client, options, req) {
        return true;
    }

    onJoin (client: Client, options: any) {
        // lock room connection possibility for new client when room is full
        if (this.clients.length > 1)
            this.lock();

        client.send("hello", "world");
        this.state.createPlayer(client.sessionId, options);
    }

    onLeave (client) {
        this.state.removePlayer(client.sessionId);
    }

    onDispose () {
        console.log("Dispose StateHandlerRoom");
    }

}
