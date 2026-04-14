export interface Comment {
  id: number;
  text: string;
  userName: string;
  createdAt: string;
  parentId?: number;
  imageId?: string;
  textFileId: string;
  textFileName?: string;
  imageOriginalUrl?: string;
  replyCount: number;
}