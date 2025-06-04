import Recurrence from "../models/recurrence";
import Sqlitedb from "./sqlitedb";

export default class RecurrenceRepository {

    private db: Sqlitedb
    private tableName: string = 'recurrences'

    constructor() {
        this.db = new Sqlitedb();
    }

    async getAllActive(): Promise<Recurrence[]> {
        const entities = await this.db.getAll<Recurrence>(this.tableName)
        return entities.filter(entity => entity.isActive)
    }
}