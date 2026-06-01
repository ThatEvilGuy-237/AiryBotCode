// Placeholder data for the Database browser. There is no data API on the backend
// yet (only the settings + auth endpoints), so this view runs on local mock rows.
// Swap these arrays for `api` calls once read endpoints exist.

export interface Column {
  key: string
  label: string
}

export interface TableDef {
  id: string
  name: string
  columns: Column[]
  rows: Record<string, string | number>[]
}

export const tables: TableDef[] = [
  {
    id: 'chatUsers',
    name: 'Chat Users',
    columns: [
      { key: 'id', label: 'ID' },
      { key: 'userName', label: 'Username' },
      { key: 'aiOpinion', label: 'AI Opinion' },
    ],
    rows: [
      { id: 1001, userName: 'TestUser1', aiOpinion: 'A friendly user.' },
      { id: 1002, userName: 'TestUser2', aiOpinion: 'A curious user.' },
    ],
  },
  {
    id: 'messages',
    name: 'Messages',
    columns: [
      { key: 'id', label: 'ID' },
      { key: 'userId', label: 'User ID' },
      { key: 'context', label: 'Content' },
    ],
    rows: [
      { id: 2001, userId: 1001, context: 'Hello, world!' },
      { id: 2002, userId: 1002, context: 'How are you?' },
    ],
  },
  {
    id: 'giveaways',
    name: 'Giveaways',
    columns: [
      { key: 'id', label: 'User ID' },
      { key: 'userName', label: 'Username' },
    ],
    rows: [{ id: 1001, userName: 'TestUser1' }],
  },
]
