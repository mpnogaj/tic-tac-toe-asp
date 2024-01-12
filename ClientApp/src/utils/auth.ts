import Endpoints from '@/endpoints';
import axios from 'axios';

export const isPlayer = async () => {
	try {
		await axios.get(Endpoints.PingPlayer);
		return true;
	} catch (err) {
		return false;
	}
};

// export const isInRoom = async roomGuid => {};

export const isAuth = async () => {
	try {
		await axios.get(Endpoints.PingAuth);
		return true;
	} catch (err) {
		return false;
	}
};
