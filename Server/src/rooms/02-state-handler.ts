import { Room, Client } from "colyseus";
import { Schema, type, MapSchema } from "@colyseus/schema";

export class Player extends Schema {
    @type("uint8")
    skin = 0;

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
    pX = 0;
    @type("number")
    pY = 0;
    @type("number")
    pZ = 0;
    
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

    createPlayer(sessionId: string, options: any, skin: number) {
        const player = new Player();
        player.skin = skin;
        player.speed = options.speed;
        player.maxHp = options.maxHp;
        player.currentHp = options.maxHp;
        player.pX = options.pX;
        player.pY = options.pY;
        player.pZ = options.pZ;
        player.rY = options.rY;

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
    spawnPointsCount = 1;
    skins: number[] = [0];

    mixArray(array){
        let currentIndex = array.length,  randomIndex;

        // While there remain elements to shuffle.
        while (currentIndex != 0) {
    
        // Pick a remaining element.
        randomIndex = Math.floor(Math.random() * currentIndex);
        currentIndex--;
    
        // And swap it with the current element.
        [array[currentIndex], array[randomIndex]] = [
            array[randomIndex], array[currentIndex]];
    }
  
    return array;
    }

    onCreate (options) {
        console.log("StateHandlerRoom created!", options);

        this.setState(new State());
        this.spawnPointsCount = options.spCount;
        for (var i = 0; i < options.skinsCount; i++){
            this.skins.push(i);
        }
        this.mixArray(this.skins);

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

                for (let i = 0; i < this.clients.length; i++) {
                    if (this.clients[i].id == damagedClientId){
                        
                        const point = Math.floor(Math.random() * this.spawnPointsCount);
                        this.clients[i].send("Restart", point);
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

        const skin = this.skins[this.clients.length - 1];
        this.state.createPlayer(client.sessionId, options, skin);
    }

    onLeave (client) {
        this.state.removePlayer(client.sessionId);
    }

    onDispose () {
        console.log("Dispose StateHandlerRoom");
    }

}
