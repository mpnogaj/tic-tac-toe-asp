import { Dictionary } from '@/types/other';

import Player from './player';

type Room = {
	guid: string;
	roomName: string;
	maxPlayerCount: number;
	players: Player[];
	readyDict: Dictionary<boolean>;
};

export default Room;
