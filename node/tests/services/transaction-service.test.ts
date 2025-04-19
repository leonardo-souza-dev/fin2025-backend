import TransactionService from '../../src/services/transaction-service'
import Transaction from '../../src/models/transaction'
import TransferRepository from '../../src/infra/transfer-repository'
import SimpleTransactionService from '../../src/services/simple-transaction-service'
import SimpleTransaction from '../../src/models/simple-transaction'
import RecurrenceService from '../../src/services/recurrence-service'
import Transfer from '../../src/models/transfer'

jest.mock('../../src/services/simple-transaction-service')
jest.mock('../../src/infra/transfer-repository')

describe('TransactionService - getAll', () => {

    let transactionService: TransactionService;

    let simpleTransactionServiceMock: jest.Mocked<SimpleTransactionService>;
    let transferRepositoryMock: jest.Mocked<TransferRepository>;
    let recurrenceServiceMock: jest.Mocked<RecurrenceService>;

    beforeEach(() => {

        simpleTransactionServiceMock = {
            getAllActive: jest.fn(),
            getByIdActive: jest.fn(),
            upsert: jest.fn(),
            delete: jest.fn()
        } as unknown as jest.Mocked<SimpleTransactionService>;

        transferRepositoryMock = {
            getAll: jest.fn(),
            getById: jest.fn(),
            create: jest.fn(),
            update: jest.fn(),
            delete: jest.fn()
        } as unknown as jest.Mocked<TransferRepository>;

        recurrenceServiceMock = {
            getAllActive: jest.fn(),
        } as unknown as jest.Mocked<RecurrenceService>;

        transactionService = new TransactionService(
            simpleTransactionServiceMock,
            transferRepositoryMock,
            recurrenceServiceMock
        );
    });

    afterEach(() => {
        jest.clearAllMocks();
    });


    


    it('should return transactions from simple transactions and transfers', async () => {
        // Arrange
        simpleTransactionServiceMock.getAllActive.mockResolvedValue([
            { date: '2023-01-01', description: 'Simple 1', accountId: 1, amount: 100, isActive: true, isRecurrent: false, id: 1 },
            { date: '2023-01-02', description: 'Simple 2', accountId: 2, amount: 200, isActive: true, isRecurrent: false, id: 2 },
        ]);

        transferRepositoryMock.getAll.mockResolvedValue([
            { date: '2023-01-03', description: 'Transfer 1', sourceAccountId: 1, destinationAccountId: 2, amount: 300, isActive: true, isRecurrent: false, id: 3 },
            { date: '2023-01-04', description: 'Transfer 2', sourceAccountId: 2, destinationAccountId: 3, amount: 400, isActive: false, isRecurrent: false, id: 4 },
        ]);

        // Act
        const transactions = await transactionService.getAllActive("1/2023");

        // Assert
        expect(transactions).toHaveLength(4);

        expect(transactions).toContainEqual(
            new Transaction('2023-01-01', 'Simple 1', 1, 100, 'SIMPLE', null, true, false, 1)
        );
        expect(transactions).toContainEqual(
            new Transaction('2023-01-02', 'Simple 2', 2, 200, 'SIMPLE', null, true, false, 2)
        );
        expect(transactions).toContainEqual(
            new Transaction('2023-01-03', 'Transfer 1', 1, 300, 'TRANSFER', 2, true, false, 3)
        )
    });





    it('should handle empty simple transactions and transfers', async () => {
        // Arrange
        simpleTransactionServiceMock.getAllActive.mockResolvedValue([])
        transferRepositoryMock.getAll.mockResolvedValue([])

        // Act
        const transactions = await transactionService.getAllActive("1/2023");

        // Assert
        expect(transactions).toHaveLength(0)
    });





    it('should only include active simple transactions', async () => {
        // Arrange
        const simpleTransactionsActive: SimpleTransaction[] = [
            new SimpleTransaction('2023-01-01', 'Simple 1', 1, 10000, true, false, 1)
        ]
        simpleTransactionServiceMock.getAllActive.mockResolvedValueOnce(simpleTransactionsActive)

        transferRepositoryMock.getAll.mockResolvedValue([
            { date: '2023-01-04', description: 'Transfer 2', sourceAccountId: 2, amount: 400, destinationAccountId: 3, isActive: false, isRecurrent: false, id: 4 },
        ])

        // Act
        const transactions = await transactionService.getAllActive("1/2023");

        // Assert
        expect(transactions).toHaveLength(1);
    });





    it('should only include active transfers', async () => {
        // Arrange
        simpleTransactionServiceMock.getAllActive.mockResolvedValue([]);
        transferRepositoryMock.getAll.mockResolvedValue([
            { date: '2023-01-03', description: 'Transfer 1', sourceAccountId: 1, amount: 300, destinationAccountId: 2, isActive: true, isRecurrent: false, id: 3 },
            { date: '2023-01-04', description: 'Transfer 2', sourceAccountId: 2, amount: 400, destinationAccountId: 3, isActive: false, isRecurrent: false, id: 4 },
        ])

        // Act
        const transactions = await transactionService.getAllActive("1/2023");

        // Assert
        expect(transactions).toHaveLength(2);
    });




});

