import Transfer from '../models/transfer'
import Sqlitedb from './sqlitedb'

export default class TransferRepository {

    private db: Sqlitedb
    private tableName: string = 'transfers'

    constructor() {
        this.db = new Sqlitedb();
    }

    async getAll(): Promise<Transfer[]> {
        const entities = await this.db.getAll<Transfer>(this.tableName)
        return entities
    }

    async getById(id: number): Promise<Transfer | null> {
        const entity = await this.db.getById<Transfer>(this.tableName, id)
        return entity
    }

    async create(entity: Transfer): Promise<number> {
        const id = await this.db.create(this.tableName, entity)
        return id
    }

    async update(entity: Transfer): Promise<void> {
        await this.db.update(this.tableName, entity)
    }
}