import SimpleTransaction from "../models/simple-transaction"
import Sqlitedb from "./sqlitedb";

export default class SimpleTransactionRepository {

    private db: Sqlitedb
    private tableName: string = 'simpleTransactions'

    constructor() {
        this.db = new Sqlitedb();
    }

    async getAll(): Promise<SimpleTransaction[]> {
        const entities = await this.db.getAll<SimpleTransaction>(this.tableName)
        return entities
    }

    async getById(id: number): Promise<SimpleTransaction | null> {
        const entity = await this.db.getById<SimpleTransaction>(this.tableName, id)
        return entity
    }

    async update(entity: SimpleTransaction): Promise<void> {
        await this.db.update(this.tableName, entity)
    }

    async create(entity: SimpleTransaction): Promise<number> {
        const id = await this.db.create(this.tableName, entity)
        return id
    }
}