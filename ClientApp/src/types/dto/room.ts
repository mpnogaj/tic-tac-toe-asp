import Player from './player';
import {Dictionary} from "@/types/other";

type Room = {
	guid: string;
	roomName: string;
	maxPlayerCount: number;
	players: Player[];
	readyDict: Dictionary<boolean>;
};

export default Room;
